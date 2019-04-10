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

    public static async ETTask ExcuteActiveSkill(ActiveSkillData activeSkillData)
    {
        await ExcuteSkillData(activeSkillData);      
    }

    public static async void ExcutePassiveSkill(PassiveSkillData passiveSkillData)
    {
        await  ExcuteSkillData(passiveSkillData);
    }

    static async ETTask ExcuteSkillData(BaseSkillData skillData)
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
                    if (node.Value.GetTriggerType() == Pipeline_TriggerType.碰撞检测)
                        node = node.Next;
                    node = await ExcutePipeLineData(node, skillData);
                }

                //执行完了要清理
                skillData.buffReturnValues.Clear();

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
    static async ETTask<LinkedListNode<BasePipelineData>> ExcutePipeLineData(LinkedListNode<BasePipelineData> node,BaseSkillData parentSkillData)
    {
        if (node == null) return null;
        switch (node.Value)
        {
            case Pipeline_FixedTime pipeline_DelayTime:
                if (pipeline_DelayTime.delayTime > 0)
                    await TimerComponent.Instance.WaitAsync(pipeline_DelayTime.delayTime * parentSkillData.skillExcuteSpeed);
                if (node.Next.Value.GetTriggerType() == Pipeline_TriggerType.碰撞检测)
                {
                    Pipeline_Collision pipeline_Collision = node.Next.Value as Pipeline_Collision;
                    parentSkillData.buffReturnValues.TryGetValue(pipeline_Collision.buffSignal,out var returnValue);
                    HandleBuffsWithCollision(pipeline_DelayTime, pipeline_Collision.buffSignal, node.Next, returnValue).Coroutine();
                }
                else
                {
                    HandleBuffs(pipeline_DelayTime,null);
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
                        if (startNode.Value.GetTriggerType() != Pipeline_TriggerType.碰撞检测)
                            startNode = await ExcutePipeLineData(startNode, parentSkillData);
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
            case Pipeline_SkillEnd pipeline_SkillEnd:
                //结束节点,不往下执行了
                if (pipeline_SkillEnd.duration > 0)
                {
                    await TimerComponent.Instance.WaitAsync(pipeline_SkillEnd.duration);
                }
               
                break;
            default:
                break;
        }

        return null;
    }

    static void ExcutePipeLineDataWithCollision(LinkedListNode<BasePipelineData> node, List<IBuffReturnedValue> buffReturnedValues)
    {
        Pipeline_Collision collision = node.Value as Pipeline_Collision;
        if (node.Next.Value.GetTriggerType() == Pipeline_TriggerType.碰撞检测)
        {
            //这种一般是类似于发射一个火球,火球分裂成几个小火球这种情况
            HandleBuffsWithCollision(collision, collision.buffSignal, node.Next, buffReturnedValues).Coroutine();
        }
        else
        {
            HandleBuffs(collision,buffReturnedValues);
        }
    }

    static void HandleBuffs(PipelineDataWithBuff pipelineData,List<IBuffReturnedValue> buffReturnedValues)
    {
        if (pipelineData.buffs.Count > 0)
        {
            foreach (var v in pipelineData.buffs)
            {
                if (!v.enable) continue;
             

                HandleBuff(v, buffReturnedValues).Coroutine();
            }
        }

    }

    static async ETVoid HandleBuffsWithCollision(PipelineDataWithBuff pipelineData,string aimBuffSignal, LinkedListNode<BasePipelineData> nextNode, List<IBuffReturnedValue> buffReturnedValues)
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
                        List<IBuffReturnedValue> returnValue_targets = null;
                        if (!v.ParentSkillData.buffReturnValues.TryGetValue(v.buffSignal, out returnValue_targets))
                        {
                            returnValue_targets = new List<IBuffReturnedValue>();
                            returnValue_targets.Add(new BuffReturnedValue_TargetUnit() { target = v.ParentSkillData.SourceUnit, playSpeedScale = v.ParentSkillData.skillExcuteSpeed });
                        }
                        buffActionWithCollision.ActionHandle(v.buffData, v.ParentSkillData.SourceUnit, returnValue_targets, (targetId) =>
                        {
                            List<IBuffReturnedValue> returnVars = new List<IBuffReturnedValue>();
                            returnVars.Add(new BuffReturnedValue_TargetUnit() { target = UnitComponent.Instance.Get(targetId) , playSpeedScale = v.ParentSkillData.skillExcuteSpeed });
                            ExcutePipeLineDataWithCollision(nextNode, returnVars);
                        });
                        continue;
                    }
                    else
                    {
                        Log.Error("配置错误,碰撞检测节点接收的参数来源错误! " + v.ParentSkillData.skillId + "  " + v.buffSignal);
                    }
                }
                HandleBuff(v, buffReturnedValues).Coroutine();
            }
        }

    }

    static async ETVoid HandleBuff(BuffInSkill buff, List<IBuffReturnedValue> buffReturnedValues)
    {
        if (buff.delayTime > 0)
            await TimerComponent.Instance.WaitAsync(buff.delayTime * buff.ParentSkillData.skillExcuteSpeed);
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(buff.buffData.GetBuffIdType());
        if (buffReturnedValues == null && !string.IsNullOrEmpty(buff.buffSignal_GetInput))
        {
            buff.ParentSkillData.buffReturnValues.TryGetValue(buff.buffSignal_GetInput, out buffReturnedValues);
        }
        if (buffReturnedValues == null)
        {
            buff.ParentSkillData.buffReturnValues.TryGetValue(buff.buffSignal, out buffReturnedValues);
        }
        if (buffReturnedValues == null)
        {
            buffReturnedValues = new List<IBuffReturnedValue>();
            buffReturnedValues.Add(new BuffReturnedValue_TargetUnit() { target = buff.ParentSkillData.SourceUnit, playSpeedScale = buff.ParentSkillData.skillExcuteSpeed });
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
            IBuffReturnedValue newBuffReturnedValue = buffActionWithSetOutputHandler.ActionHandle(buff.buffData, buff.ParentSkillData.SourceUnit, buffReturnedValues);
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

