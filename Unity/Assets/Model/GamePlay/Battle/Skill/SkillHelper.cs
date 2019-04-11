using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using static BaseSkillData;

public static class SkillHelper
{

    public static bool CheckSkillCanUse(ActiveSkillData skillData, Unit unit)
    {
        if (!CheckActiveConditions(skillData, unit)) return false;
        return true;
    }

    public static bool CheckActiveConditions(BaseSkillData baseSkillData, Unit source)
    {
        if (baseSkillData.activeConditionDatas.Count == 0)
            return true;
        foreach (var v in baseSkillData.activeConditionDatas)
        {
            var handler = SkillActiveConditionHandlerComponent.Instance.GetHandler(v.GetBuffActiveConditionType());
            if (!handler.MeetCondition(v, source))
            {
                return false;
            }
        }
        return true;
    }

    public static async ETTask ExcuteActiveSkill(ActiveSkillData activeSkillData, ETCancellationTokenSource tokenSource)
    {
        await ExcuteSkillData(activeSkillData, tokenSource);      
    }

    public static async void ExcutePassiveSkill(PassiveSkillData passiveSkillData, ETCancellationTokenSource tokenSource)
    {
        await  ExcuteSkillData(passiveSkillData, tokenSource);
    }

    static async ETTask ExcuteSkillData(BaseSkillData skillData,ETCancellationTokenSource  tokenSource)
    {
        try
        {
            if (skillData.pipelineDatas != null && skillData.pipelineDatas.Count > 0)
            {
                //创建一个只有所有开启的PipelineData的LinkedList,方便执行
                LinkedList<BasePipelineData> enablePipelineDataList = new LinkedList<BasePipelineData>();
                var node = skillData.pipelineDatas.First;
                while (node != null)
                {
                    if (node.Value.enable)
                    {
                        enablePipelineDataList.AddLast(new LinkedListNode<BasePipelineData>(node.Value));
                    }
                    node = node.Next;
                }
                node = enablePipelineDataList.First;


                while (node != null)
                {
                    if (tokenSource.Token.IsCancellationRequested) return; //如果后续节点的执行已经取消了,那么这里就返回不执行了
                    if (node.Value.GetTriggerType() == Pipeline_TriggerType.碰撞检测)
                        node = node.Next;
                    node = await ExcutePipeLineData(node, skillData, tokenSource);
                    tokenSource?.Dispose();
                    tokenSource = new ETCancellationTokenSource();
                }

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
    static async ETTask<LinkedListNode<BasePipelineData>> ExcutePipeLineData(LinkedListNode<BasePipelineData> node,BaseSkillData parentSkillData,ETCancellationTokenSource tokenSource)
    {
        if (node == null) return null;
        switch (node.Value)
        {
            case Pipeline_FixedTime pipeline_DelayTime:
                
                if (pipeline_DelayTime.delayTime > 0)
                    await TimerComponent.Instance.WaitAsync((long)(pipeline_DelayTime.delayTime * parentSkillData.skillExcuteSpeed* 1000), tokenSource.Token);
                if (node.Next.Value.GetTriggerType() == Pipeline_TriggerType.碰撞检测)
                {
                    Pipeline_Collision pipeline_Collision = node.Next.Value as Pipeline_Collision;
                    parentSkillData.buffReturnValues.TryGetValue(pipeline_Collision.buffSignal,out var returnValue);
                    HandleBuffsWithCollision(pipeline_DelayTime, pipeline_Collision.buffSignal, node.Next, returnValue, tokenSource).Coroutine();
                }
                else
                {
                    HandleBuffs(pipeline_DelayTime,null, tokenSource).Coroutine();
                }
                if (pipeline_DelayTime.fixedTime > 0)
                    await TimerComponent.Instance.WaitAsync(pipeline_DelayTime.fixedTime * parentSkillData.skillExcuteSpeed);
                return node.Next;
            case Pipeline_Collision pipeline_Collision:
                //这里不直接处理 
               
                return node.Next;
            case Pipeline_CycleStart pipeline_CycleStart:

                for (int i = 0; i < pipeline_CycleStart.repeatCount; i++)
                {
                    var startNode = node.Next;
                    for (int j = 0; j < pipeline_CycleStart.cycleRange; j++)
                    {
                        if (tokenSource.Token.IsCancellationRequested) return null;
                        if (startNode.Value.GetTriggerType() != Pipeline_TriggerType.碰撞检测)
                            startNode = await ExcutePipeLineData(startNode, parentSkillData, tokenSource);
                        else
                            startNode = startNode.Next;
                    }
                }

                var nextNode = node.Next;
                for (int i = 0; i < pipeline_CycleStart.cycleRange; i++)
                {
                    nextNode = nextNode.Next;
                }

                return nextNode;
            default:
                break;
        }

        return null;
    }

    static void ExcutePipeLineDataWithCollision(LinkedListNode<BasePipelineData> node, List<IBufferValue> buffReturnedValues, ETCancellationTokenSource tokenSource)
    {
        Pipeline_Collision collision = node.Value as Pipeline_Collision;
        if (node.Next.Value.GetTriggerType() == Pipeline_TriggerType.碰撞检测)
        {
            //这种一般是类似于发射一个火球,火球分裂成几个小火球这种情况
            HandleBuffsWithCollision(collision, collision.buffSignal, node.Next, buffReturnedValues, tokenSource).Coroutine();
        }
        else
        {
            HandleBuffs(collision,buffReturnedValues, tokenSource).Coroutine();
        }
    }

    static async ETVoid HandleBuffs(PipelineDataWithBuff pipelineData,List<IBufferValue> buffReturnedValues, ETCancellationTokenSource tokenSource)
    {
        if (pipelineData.buffs.Count > 0)
        {
            foreach (var v in pipelineData.buffs)
            {
                if (!v.enable) continue;
                if (v.delayTime > 0)
                    await TimerComponent.Instance.WaitAsync((long)(1000 * v.delayTime), tokenSource.Token);
                HandleBuff(v, buffReturnedValues);
            }
        }

    }

    static async ETVoid HandleBuffsWithCollision(PipelineDataWithBuff pipelineData,string aimBuffSignal, LinkedListNode<BasePipelineData> nextNode, List<IBufferValue> buffReturnedValues, ETCancellationTokenSource tokenSource)
    {
        if (pipelineData.buffs.Count > 0)
        {
            foreach (var v in pipelineData.buffs)
            {
                if (!v.enable) continue;
                if (v.delayTime > 0)
                    await TimerComponent.Instance.WaitAsync(v.delayTime  * v.ParentSkillData.skillExcuteSpeed);
                BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.buffData.GetBuffIdType());
                if (v.buffSignal == aimBuffSignal)
                {
                    IBuffActionWithCollision buffActionWithCollision = baseBuffHandler as IBuffActionWithCollision;
                    if (buffActionWithCollision != null)
                    {
                        List<IBufferValue> returnValue_targets = null;
                        if (!v.ParentSkillData.buffReturnValues.TryGetValue(v.buffSignal, out returnValue_targets))
                        {
                            returnValue_targets = new List<IBufferValue>();
                            returnValue_targets.Add(new BufferValue_TargetUnits() { target = v.ParentSkillData.SourceUnit, playSpeedScale = v.ParentSkillData.skillExcuteSpeed });
                        }
                        buffActionWithCollision.ActionHandle(v.buffData, v.ParentSkillData.SourceUnit, returnValue_targets, (targetId) =>
                        {
                            List<IBufferValue> returnVars = new List<IBufferValue>();
                            returnVars.Add(new BufferValue_TargetUnits() { target = UnitComponent.Instance.Get(targetId) , playSpeedScale = v.ParentSkillData.skillExcuteSpeed });
                            ExcutePipeLineDataWithCollision(nextNode, returnVars, tokenSource);
                        });
                        continue;
                    }
                    else
                    {
                        Log.Error("配置错误,碰撞检测节点接收的参数来源错误! " + v.ParentSkillData.skillId + "  " + v.buffSignal);
                    }
                }
                HandleBuff(v, buffReturnedValues);
            }
        }

    }

    static void HandleBuff(BuffInSkill buff, List<IBufferValue> buffReturnedValues)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(buff.buffData.GetBuffIdType());
        if (buffReturnedValues == null && !string.IsNullOrEmpty(buff.signal_GetInput))
        {
            buff.ParentSkillData.buffReturnValues.TryGetValue(buff.signal_GetInput, out buffReturnedValues);
        }
        if (buffReturnedValues == null)
        {
            buff.ParentSkillData.buffReturnValues.TryGetValue(buff.buffSignal, out buffReturnedValues);
        }
        if (buffReturnedValues == null)
        {
            buffReturnedValues = new List<IBufferValue>();
            buffReturnedValues.Add(new BufferValue_TargetUnits() { target = buff.ParentSkillData.SourceUnit, playSpeedScale = buff.ParentSkillData.skillExcuteSpeed });
        }
        IBuffActionWithGetInputHandler buffActionWithGetInput = baseBuffHandler as IBuffActionWithGetInputHandler;
        if (buffActionWithGetInput != null)
        {
            buffActionWithGetInput.ActionHandle(buff.buffData, buff.ParentSkillData.SourceUnit, buffReturnedValues);
            return;
        }
        IBuffActionWithSetOutputHandler buffActionWithSetOutputHandler = baseBuffHandler as IBuffActionWithSetOutputHandler;
        if (buffActionWithSetOutputHandler != null)
        {
            List<IBufferValue> newBuffReturnedValue = buffActionWithSetOutputHandler.ActionHandle(buff.buffData, buff.ParentSkillData.SourceUnit, buffReturnedValues);
            buff.ParentSkillData.AddReturnValue(buff.buffSignal, newBuffReturnedValue);
            return;
        }

    }

    #region 获取技能的目标

   
    #endregion
    #region 自动选择技能
    public static ETTaskCompletionSource<ActiveSkillData> ChooseSkillDataTask;
    public static ActiveSkillData GetActiveSkillData(Unit unit)
    {
        try
        {
            ActiveSkillData activeSkillData = unit.GetComponent<AutoChoosedSkillComponent>().GetSkillData(unit);
            Log.Debug(activeSkillData.skillName);
            return activeSkillData;

        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
            return null;
        }
    }
    #endregion
}

