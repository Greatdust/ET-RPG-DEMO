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
    //存储技能执行过程中产生的中间数据,一般在切换场景的时候清理一下就好
    public static Dictionary<(Unit,string), Dictionary<Type, IBufferValue>> tempData = new Dictionary<(Unit, string), Dictionary<Type, IBufferValue>>();

    

    public static bool CheckIfSkillCanUse(string skillId, Unit source)
    {
        CharacterStateComponent characterStateComponent = source.GetComponent<CharacterStateComponent>();
        if (characterStateComponent.Get(SpecialStateType.NotInControl)) return false;

        var skillData = GetBaseSkillData(skillId);

        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer((source, skillId).GetHashCode());

        if (TimeHelper.ClientNow() - timer.timing < timer.interval)
        {
           // Log.Debug("技能还在冷却中!!");
            return false;
        }
        timer.interval = (long)(skillData.coolDown * 1000);

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

    public static BaseSkillData GetBaseSkillData(string skillId)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();

        BaseSkillData skillData = skillConfigComponent.GetActiveSkill(skillId);
        if (skillData == null)
        {
            skillData = skillConfigComponent.GetPassiveSkill(skillId);

        }
        return skillData;
    }

    public struct ExecuteSkillParams : IDisposable
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
            cycleStartsStack?.Clear();
            cycleStartsStack = null;
        }
    }
 


    public static async ETTask ExecuteActiveSkill(ExecuteSkillParams skillParams)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();

        skillParams.baseSkillData = skillConfigComponent.GetActiveSkill(skillParams.skillId);
        await ExecuteSkillData(skillParams);
    }

    public static async ETTask<bool> CheckInput(ExecuteSkillParams skillParams)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();

        var activeSkill = skillConfigComponent.GetActiveSkill(skillParams.skillId);
        try
        {
            if (activeSkill.pipelineDatas != null && activeSkill.pipelineDatas.Count > 0)
            {
                //创建一个只有所有开启的PipelineData的LinkedList,方便执行. 另外需要碰撞才能触发的 不会加入到执行列表里
                LinkedList<BasePipelineData> enablePipelineDataList = new LinkedList<BasePipelineData>();

                foreach (var value in activeSkill.inputCheck)
                {
                    if (value.enable)
                        enablePipelineDataList.AddLast(new LinkedListNode<BasePipelineData>(value));
                }
                var node = enablePipelineDataList.First;
                skillParams.cancelToken = new CancellationTokenSource();
                while (node != null)
                {
                    if (skillParams.cancelToken.Token.IsCancellationRequested) return false; //如果后续节点的执行已经取消了,那么这里就返回不执行了
                    node = await ExecutePipelineNode(node, skillParams);
                }
                if (skillParams.cancelToken.Token.IsCancellationRequested) return false;
                
                skillParams.Dispose();
                return true;
            }
            Log.Error("技能没有配置输入检测阶段 " + skillParams.skillId);
            return false; // 没有输入检测阶段,这是配置错了.
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            return false;
        }
    }

    public static async void ExecutePassiveSkill(ExecuteSkillParams skillParams)
    {
        SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();

        skillParams.baseSkillData = skillConfigComponent.GetPassiveSkill(skillParams.skillId);
        await ExecuteSkillData(skillParams);
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

    static async ETTask ExecuteSkillData(ExecuteSkillParams skillParams)
    {
        try
        {
            skillParams.playSpeed = 1;
            skillParams.cycleStartsStack = new Stack<(LinkedListNode<BasePipelineData>, int, int)>();

            if (skillParams.baseSkillData.pipelineDatas != null && skillParams.baseSkillData.pipelineDatas.Count > 0)
            {
                //创建一个只有所有开启的PipelineData的LinkedList,方便执行.
                LinkedList<BasePipelineData> enablePipelineDataList = new LinkedList<BasePipelineData>();

                foreach(var value in skillParams.baseSkillData.pipelineDatas)
                {
                    if (value.enable )
                    {
                        enablePipelineDataList.AddLast(new LinkedListNode<BasePipelineData>(value));
                    }
                }
                var node = enablePipelineDataList.First;

                //进入技能的执行阶段了,被打断就要计算冷却了.
                skillParams.cancelToken.Token.Register(() =>
                {
                    CalCoolDown(skillParams);
                });
                while (node != null)
                {
                    if (skillParams.cancelToken.Token.IsCancellationRequested) return; //如果后续节点的执行已经取消了,那么这里就返回不执行了
                    node = await ExecutePipelineNode(node, skillParams);
                }
                //技能使用结束,在这里计算冷却
                CalCoolDown(skillParams);
                //TODO; 发出消息提示进入冷却
                //
                skillParams.Dispose();
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

    }

    static void CalCoolDown(ExecuteSkillParams skillParams)
    {
        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer((skillParams.source, skillParams.skillId).GetHashCode());
        timer.interval = (long)(skillParams.baseSkillData.coolDown * 1000);
        timer.timing = TimeHelper.ClientNow();
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="node"></param>
    /// <returns></returns>
    static async ETTask<LinkedListNode<BasePipelineData>> ExecutePipelineNode(LinkedListNode<BasePipelineData> node, ExecuteSkillParams skillParams)
    {
        if (node == null) return null;

        BuffHandlerVar buffHandlerVar = new BuffHandlerVar();
        buffHandlerVar.playSpeed = skillParams.playSpeed;
        buffHandlerVar.skillId = skillParams.skillId;
        buffHandlerVar.skillLevel = skillParams.skillLevel;
        buffHandlerVar.source = skillParams.source;
        switch (node.Value)
        {
            case Pipeline_FindTarget findTarget:
                findTarget.Execute(skillParams);
                return node.Next;

            case Pipeline_WaitForInput pipeline_WaitForInput:
                bool result = await pipeline_WaitForInput.Execute(skillParams);
                if (!result)
                {
                    skillParams.cancelToken?.Cancel();
                    return null;
                }

                return node.Next;
            case Pipeline_FixedTime pipeline_DelayTime:
                await pipeline_DelayTime.Execute(skillParams, buffHandlerVar);
                return node.Next;

            case Pipeline_CycleStart pipeline_CycleStart:
                pipeline_CycleStart.Execute(node, skillParams);
                return node.Next;
            case Pipeline_CycleEnd pipeline_CycleEnd:
                return await pipeline_CycleEnd.Execute(node, skillParams);
            case Pipeline_Programmable pipeline_Programmable:
                pipeline_Programmable.Execute(skillParams);
                return node.Next;
            case Pipeline_ApplyData pipeline_ApplyData:
#if !SERVER
                //联网模式的游戏,客户端不直接进行结算效果的执行
                if (GlobalConfigComponent.Instance.networkPlayMode)
                {
                    return node.Next;
                }
#endif
                ExecuteApplyData(pipeline_ApplyData.applyPipelineSignal, skillParams.baseSkillData, buffHandlerVar).Coroutine();
                return node.Next;
            default:
                break;
        }

        return null;
    }

#if !SERVER

    //联网模式下,客户端进行结算效果处理的时,只结算处理特效,声音等的播放.
    //注意,这里面可能需要一些数据,应该提前填充好.比如范围检测到哪些目标,服务器下发后在这个函数调用之前,填充完毕
    public static async ETVoid ExecuteApplyData_Client(string applyPipelineSignal, Unit source, string skillId)
    {
        try
        {

            foreach (var applyData in GetBaseSkillData(skillId).applyDatas)
            {
                if (applyData.pipelineSignal == applyPipelineSignal)
                {

                    switch (applyData)
                    {
                        //结算节点暂时就支持这一种
                        case Pipeline_FixedTime pipeline_DelayTime:
                            if (pipeline_DelayTime.buffs.Count > 0)
                            {
                                foreach (var v in pipeline_DelayTime.buffs)
                                {
                                    if (!v.enable) continue;
                                    switch (v.buffData.GetBuffIdType())
                                    {
                                        case BuffIdType.EmitObj:
                                        case BuffIdType.HitEffect:
                                        case BuffIdType.PlayAnim:
                                        case BuffIdType.PlayEffect:
                                        case BuffIdType.PlaySound:
                                            if (v.delayTime > 0)
                                                await TimerComponent.Instance.WaitAsync(v.delayTime); // 结算阶段,技能执行速度不受技能释放速度影响
                                            BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.buffData.GetBuffIdType());

                                            BuffHandlerVar newVar = new BuffHandlerVar();
                                            newVar.playSpeed = 1;
                                            newVar.source = source;
                                            newVar.skillId = skillId;
                                            newVar.skillLevel = 1;//如果游戏需要根据不同的技能等级播放不同的特效,那么传递的参数请增加一个技能等级,这里默认填1了
                                            newVar.cancelToken = new CancellationToken();
 
                                            newVar.bufferValues = new Dictionary<Type, IBufferValue>();

                                            HandleBuff(v, newVar);
                                            break;
                                    }
                                
                                }
                            }
                            break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
#endif

    public static async ETVoid ExecuteApplyData(string applyPipelineSignal, BaseSkillData baseSkillData, BuffHandlerVar buffHandlerVar)
    {
        try
        {
            foreach (var applyData in baseSkillData.applyDatas)
            {
                if (applyData.pipelineSignal == applyPipelineSignal)
                {

                    switch (applyData)
                    {
                        //结算节点暂时就支持这一种
                        case Pipeline_FixedTime pipeline_DelayTime:
                            if (pipeline_DelayTime.buffs.Count > 0)
                            {
                                foreach (var v in pipeline_DelayTime.buffs)
                                {
                                    if (!v.enable) continue;
                                    if (v.delayTime > 0)
                                        await TimerComponent.Instance.WaitAsync(v.delayTime); // 结算阶段,技能执行速度不受技能释放速度影响
                                    BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.buffData.GetBuffIdType());

                                    BuffHandlerVar newVar = buffHandlerVar;

                                    newVar.bufferValues = new Dictionary<Type, IBufferValue>();
                                    if (buffHandlerVar.bufferValues != null)
                                        foreach (var buffValue in buffHandlerVar.bufferValues)
                                        {
                                            newVar.bufferValues[buffValue.Key] = buffValue.Value;
                                        }

                                    HandleBuff(v, newVar);
                                }
                            }
                            break;
                    }
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

    }


    public static async ETVoid HandleBuffs(PipelineDataWithBuff pipelineData, BuffHandlerVar buffHandlerVar, ExecuteSkillParams skillParams)
    {
        if (pipelineData.buffs.Count > 0)
        {
            foreach (var v in pipelineData.buffs)
            {
                if (!v.enable) continue;
                if (v.delayTime > 0)
                    await TimerComponent.Instance.WaitAsync((long)(1000 * v.delayTime), skillParams.cancelToken.Token);
                if (skillParams.cancelToken == null || skillParams.cancelToken.IsCancellationRequested) return;
                buffHandlerVar.cancelToken = skillParams.cancelToken.Token;
                BuffHandlerVar newVar = buffHandlerVar;
                newVar.bufferValues = new Dictionary<Type, IBufferValue>();
                if (buffHandlerVar.bufferValues != null)
                    foreach (var buffValue in buffHandlerVar.bufferValues)
                    {
                        newVar.bufferValues[buffValue.Key] = buffValue.Value;
                    }

                HandleBuff(v, newVar);
            }
        }

    }

    static void HandleBuff(BuffInSkill buff, BuffHandlerVar buffHandlerVar)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(buff.buffData.GetBuffIdType());
        if (buff.signals_GetInput != null && buff.signals_GetInput.Length > 0)
        {
            foreach (var signal in buff.signals_GetInput)
            {
                if (tempData.ContainsKey((buffHandlerVar.source,signal)))
                    foreach (var v in tempData[(buffHandlerVar.source, signal)])
                    {
                        buffHandlerVar.bufferValues[v.Key] = v.Value;
                    }
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
            if (tempData.ContainsKey((buffHandlerVar.source, buff.buffData.buffSignal)))
            {
                //可能还存在着之前使用技能时找到的数据. 所以这里清理掉它
                tempData.Remove((buffHandlerVar.source, buff.buffData.buffSignal));
            }
           

            if (newBuffReturnedValue == null)
            {
                
                return;
            } 
            if (!tempData.TryGetValue((buffHandlerVar.source, buff.buffData.buffSignal), out var Dic))
            {
                Dic = new Dictionary<Type, IBufferValue>();
                tempData[(buffHandlerVar.source, buff.buffData.buffSignal)] = Dic;
            }
            foreach (var v in newBuffReturnedValue)
            {
                Dic[v.GetType()] = v;
            }
            return;
        }

    }

}

