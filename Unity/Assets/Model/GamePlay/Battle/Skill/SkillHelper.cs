using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ETModel;
using static BaseSkillData;

public static class SkillHelper
{

    public static readonly Dictionary<string, Action<BuffHandlerVar>> collisionActions = new Dictionary<string, Action<BuffHandlerVar>>();// 存储技能执行中的飞行道具碰撞时会触发的逻辑

    //存储技能执行过程中产生的中间数据,一般在切换场景的时候清理一下就好
    public static Dictionary<string, Dictionary<Type, IBufferValue>> tempData = new Dictionary<string, Dictionary<Type, IBufferValue>>();

    

    public static bool CheckIfSkillCanUse(string skillId, Unit source)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();

        BaseSkillData skillData = skillConfigComponent.GetActiveSkill(skillId);
        if (skillData == null)
        {
            skillData = skillConfigComponent.GetPassiveSkill(skillId);

            TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(source.GetHashCode() + skillId.GetHashCode());

            if (TimeHelper.ClientNow() - timer.timing < timer.interval)
            {
                return false;
            }
            timer.interval = (long)(skillData.coolDown * 1000);
        }


        if (skillData.activeConditionDatas.Count == 0)
            return true;
        foreach (var v in skillData.activeConditionDatas)
        {
            var handler = SkillActiveConditionHandlerComponent.Instance.GetHandler(v.GetBuffActiveConditionType());
            if (!handler.MeetCondition(v, source))
            {
                return false;
            }
        }
        return true;
    }

    public struct ExcuteSkillParams : IDisposable
    {
        public Unit source; // 技能使用方
        public string skillId;
        public int skillLevel;
        public BaseSkillData baseSkillData;
        public float playSpeed; // 播放速度,影响特效,音效,动作的速度
        public CancellationTokenSource cancelToken; // 用来中断技能的
        public Stack<(LinkedListNode<BasePipelineData>, int, int)> cycleStartsStack;// 一个专门为循环开始/节点服务的栈. 第一个int是当前循环次数,第二个int是总循环次数


        public void Dispose()
        {
            cancelToken?.Dispose();
            cycleStartsStack?.Clear();
            cycleStartsStack = null;
        }
    }
 


    public static async ETTask ExcuteActiveSkill(ExcuteSkillParams skillParams)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();

        skillParams.baseSkillData = skillConfigComponent.GetActiveSkill(skillParams.skillId);
        await ExcuteSkillData(skillParams);
    }

    public static async void ExcutePassiveSkill(ExcuteSkillParams skillParams)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();

        skillParams.baseSkillData = skillConfigComponent.GetPassiveSkill(skillParams.skillId);
        await ExcuteSkillData(skillParams);
    }

    public static void OnPassiveSkillRemove(Unit unit, string skillId)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();
        PassiveSkillData passiveSkillData = skillConfigComponent.GetPassiveSkill(skillId);
        BuffHandlerVar buffHandlerVar = new BuffHandlerVar();
        buffHandlerVar.source = unit;
        buffHandlerVar.skillId = skillId;
        foreach (var v in passiveSkillData.pipelineDatas)
        {
            PipelineDataWithBuff pipelineDataWithBuff = v as PipelineDataWithBuff;


            if (pipelineDataWithBuff != null)
            {
                
                foreach (var buff in pipelineDataWithBuff.buffs)
                {
                    IBuffRemoveHanlder buffRemoveHanlder = Game.Scene.GetComponent<BuffHandlerComponent>().GetHandler(buff.buffData.GetBuffIdType()) as IBuffRemoveHanlder;
                    if (buffRemoveHanlder != null)
                    {
                        buffHandlerVar.data = buff.buffData;
                        buffRemoveHanlder.Remove(buffHandlerVar);
                    }
                }
            }
        }

    }

    static async ETTask ExcuteSkillData(ExcuteSkillParams skillParams)
    {
        try
        {
            TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(skillParams.source.GetHashCode() + skillParams.skillId.GetHashCode());
            timer.interval = (long)(skillParams.baseSkillData.coolDown * 1000);
            timer.timing = TimeHelper.ClientNow();
            //TODO; 发出消息提示进入冷却
            skillParams.playSpeed = 1;

            skillParams.cycleStartsStack = new Stack<(LinkedListNode<BasePipelineData>, int, int)>();

            if (skillParams.baseSkillData.pipelineDatas != null && skillParams.baseSkillData.pipelineDatas.Count > 0)
            {
                //创建一个只有所有开启的PipelineData的LinkedList,方便执行. 另外需要碰撞才能触发的 不会加入到执行列表里
                LinkedList<BasePipelineData> enablePipelineDataList = new LinkedList<BasePipelineData>();

                foreach(var value in skillParams.baseSkillData.pipelineDatas)
                {
                    if (value.enable && value.GetTriggerType() != Pipeline_TriggerType.碰撞检测)
                    {
                        enablePipelineDataList.AddLast(new LinkedListNode<BasePipelineData>(value));
                    }
                    if (value.GetTriggerType() == Pipeline_TriggerType.碰撞检测)
                    {
                        // 向CollisionAction中添加Action,由其他可以触发碰撞事件的buff来触发
                        if (!collisionActions.ContainsKey(value.pipelineSignal))
                        {
                            collisionActions[value.pipelineSignal] = (var) => { ExcutePipeLine_Collision(value as Pipeline_Collision, var); };
                        }

                    }
                }
                var node = enablePipelineDataList.First;

                while (node != null)
                {
                    if (skillParams.cancelToken.Token.IsCancellationRequested) return; //如果后续节点的执行已经取消了,那么这里就返回不执行了
                    node = await ExcutePipeLineData(node, skillParams);
                }

                //
                skillParams.Dispose();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    static async ETTask<LinkedListNode<BasePipelineData>> ExcutePipeLineData(LinkedListNode<BasePipelineData> node, ExcuteSkillParams skillParams)
    {
        if (node == null) return null;
        BuffHandlerVar buffHandlerVar = new BuffHandlerVar();
        buffHandlerVar.playSpeed = skillParams.playSpeed;
        buffHandlerVar.skillId = skillParams.skillId;
        buffHandlerVar.skillLevel = skillParams.skillLevel;
        buffHandlerVar.source = skillParams.source;
        switch (node.Value)
        {
            case Pipeline_FixedTime pipeline_DelayTime:

                if (pipeline_DelayTime.delayTime > 0)
                {
                    await TimerComponent.Instance.WaitAsync((long)(pipeline_DelayTime.delayTime * skillParams.playSpeed * 1000), skillParams.cancelToken.Token);

                }

                HandleBuffs(pipeline_DelayTime, buffHandlerVar, skillParams).Coroutine();

                if (pipeline_DelayTime.fixedTime > 0)
                {
                    await TimerComponent.Instance.WaitAsync((long)(pipeline_DelayTime.fixedTime * skillParams.playSpeed * 1000), skillParams.cancelToken.Token);
                }
                return node.Next;

            case Pipeline_CycleStart pipeline_CycleStart:

                (LinkedListNode<BasePipelineData>, int, int) stackItem = default;
                if (skillParams.cycleStartsStack.Count > 0)
                {
                    stackItem = skillParams.cycleStartsStack.Pop();

                }
                stackItem.Item1 = node;
                stackItem.Item2++;
                stackItem.Item3 = pipeline_CycleStart.repeatCount;
                skillParams.cycleStartsStack.Push(stackItem);

                return node.Next;
            case Pipeline_CycleEnd pipeline_CycleEnd:
                stackItem = skillParams.cycleStartsStack.Pop();
                if (stackItem.Item2 >= stackItem.Item3)
                {
                    return node.Next;
                }
                else
                {
                    skillParams.cycleStartsStack.Push(stackItem);
                    return stackItem.Item1;
                }
            case Pipeline_Programmable pipeline_Programmable:
                SkillData_Var skillData_Var = default;
                skillData_Var.pipelineSignal = pipeline_Programmable.pipelineSignal;
                skillData_Var.skillId = skillParams.skillId;
                skillData_Var.source = skillParams.source;
                skillParams.cancelToken.Token.Register(()=>pipeline_Programmable.pmb.Break(skillData_Var));
                pipeline_Programmable.pmb.Excute(skillData_Var);
                return node.Next;
            case Pipeline_FindTarget findTarget:
                switch (findTarget.findTargetType)
                {
                    case FindTargetType.自身:
                        tempData[findTarget.pipelineSignal] = new Dictionary<Type, IBufferValue>();
                        tempData[findTarget.pipelineSignal][typeof(BufferValue_TargetUnits)] = new BufferValue_TargetUnits() { targets = new Unit[] { skillParams.source } };
                        break;
                    case FindTargetType.我方N人:
                        break;
                    case FindTargetType.敌方N人:
                        break;
                    case FindTargetType.我方全体:
                        break;
                    case FindTargetType.敌方全体:
                        break;
                    case FindTargetType.自身为中心的范围内:
                        break;
                    case FindTargetType.距离自己最近的N个队友:
                        break;
                    case FindTargetType.距离自己最近的N个敌人:
                        break;
                }
                return node.Next;

            case Pipeline_WaitForInput pipeline_WaitForInput:
                switch (pipeline_WaitForInput.inputType)
                {
                    //等待用户输入,可能有正确输入/取消/输入超时三种情况

                    case InputType.Tar:

                        break;
                    case InputType.Dir:
                        //直接智能施法模式
                        BufferValue_Dir bufferValue_Dir = new BufferValue_Dir();
                        bufferValue_Dir.dir = UnitComponent.Instance.MyUnit.GetComponent<InputComponent>().GetInputDir();
                        if (!tempData.ContainsKey(pipeline_WaitForInput.pipelineSignal))
                        {
                            tempData[pipeline_WaitForInput.pipelineSignal] = new Dictionary<Type, IBufferValue>();
                        }
                        tempData[pipeline_WaitForInput.pipelineSignal][typeof(BufferValue_Dir)] = bufferValue_Dir;
                        break;
                    case InputType.Pos:
                        break;
                    case InputType.Charge:
                        break;
                    case InputType.Spell:
                        break;
                    case InputType.ContinualSpell:
                        //这个持续引导,需要一个专门的使用持续引导Effect的Handler来配合. 如果游戏内这样的技能比较少,干脆用可编程节点实现,然后抽离出数据 ,让对应节点可复用

                        break;
                }
                return node.Next;
            default:
                break;
        }

        return null;
    }

    static void ExcutePipeLine_Collision(Pipeline_Collision pipeline_Collision, BuffHandlerVar buffHandlerVar)
    {

        HandleBuffsWithCollision(pipeline_Collision, buffHandlerVar).Coroutine();

    }

    static async ETVoid HandleBuffsWithCollision(Pipeline_Collision pipelineData, BuffHandlerVar buffHandlerVar)
    {
        if (pipelineData.buffs.Count > 0)
        {
            foreach (var v in pipelineData.buffs)
            {
                if (!v.enable) continue;
                if (v.delayTime > 0)
                    await TimerComponent.Instance.WaitAsync(v.delayTime); // 这是飞行道具产生的结果, 不受技能释放速度影响
                BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.buffData.GetBuffIdType());

                BuffHandlerVar newVar = buffHandlerVar;
                if (buffHandlerVar.bufferValues != null)
                    newVar.bufferValues = new Dictionary<Type, IBufferValue>();
                foreach (var buffValue in buffHandlerVar.bufferValues)
                {
                    newVar.bufferValues[buffValue.Key] = buffValue.Value;
                }

                HandleBuff(v, buffHandlerVar);
            }
        }

    }


    static async ETVoid HandleBuffs(PipelineDataWithBuff pipelineData, BuffHandlerVar buffHandlerVar, ExcuteSkillParams skillParams)
    {
        if (pipelineData.buffs.Count > 0)
        {
            foreach (var v in pipelineData.buffs)
            {
                if (!v.enable) continue;
                if (v.delayTime > 0)
                    await TimerComponent.Instance.WaitAsync((long)(1000 * v.delayTime), skillParams.cancelToken.Token);
                BuffHandlerVar newVar = buffHandlerVar;
                newVar.bufferValues = new Dictionary<Type, IBufferValue>();
                if (buffHandlerVar.bufferValues != null)
                    foreach (var buffValue in buffHandlerVar.bufferValues)
                    {
                        newVar.bufferValues[buffValue.Key] = buffValue.Value;
                    }

                HandleBuff(v, buffHandlerVar);
            }
        }

    }

    static void HandleBuff(BuffInSkill buff, BuffHandlerVar buffHandlerVar)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(buff.buffData.GetBuffIdType());
        if (buff.signals_GetInput != null && buff.signals_GetInput.Length > 0)
        {
            foreach (var signal in buff.signals_GetInput)
                foreach (var v in tempData[signal])
                {
                    buffHandlerVar.bufferValues[v.Key] = v.Value;
                }
        }
        buffHandlerVar.data = buff.buffData;

        IBuffActionWithGetInputHandler buffActionWithGetInput = baseBuffHandler as IBuffActionWithGetInputHandler;
        if (buffActionWithGetInput != null)
        {
            buffActionWithGetInput.ActionHandle(buffHandlerVar);
            return;
        }
        IBuffActionWithSetOutputHandler buffActionWithSetOutputHandler = baseBuffHandler as IBuffActionWithSetOutputHandler;
        if (buffActionWithSetOutputHandler != null)
        {
            var newBuffReturnedValue = buffActionWithSetOutputHandler.ActionHandle(buffHandlerVar);
            if (newBuffReturnedValue == null) return;
            if (!tempData.TryGetValue(buff.buffData.buffSignal, out var Dic))
            {
                Dic = new Dictionary<Type, IBufferValue>();
                tempData[buff.buffData.buffSignal] = Dic;
            }
            foreach (var v in newBuffReturnedValue)
            {
                Dic[v.GetType()] = v;
            }
            return;
        }

    }

}

