using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class PipelineSystem_Cycle
{
    public static void Execute(this Pipeline_CycleStart pipeline_CycleStart, LinkedListNode<BasePipelineData> node, SkillHelper.ExecuteSkillParams skillParams)
    {
        (LinkedListNode<BasePipelineData>, int, int) stackItem = default;
        if (skillParams.cycleStartsStack.Count > 0)
        {
            stackItem = skillParams.cycleStartsStack.Pop();

        }
        stackItem.Item1 = node;
        stackItem.Item2++;
        stackItem.Item3 = pipeline_CycleStart.repeatCount;
        skillParams.cycleStartsStack.Push(stackItem);
    }

    public static async ETTask<LinkedListNode<BasePipelineData>> Execute(this Pipeline_CycleEnd cycleEnd, LinkedListNode<BasePipelineData> node, SkillHelper.ExecuteSkillParams skillParams)
    {
        (LinkedListNode<BasePipelineData>, int, int) stackItem = default;
        stackItem = skillParams.cycleStartsStack.Pop();
        if (stackItem.Item2 >= stackItem.Item3)
        {
            return node.Next;
        }
        else
        {
            if(cycleEnd.waitTime>0)
            await TimerComponent.Instance.WaitAsync((long)(cycleEnd.waitTime * skillParams.playSpeed * 1000), skillParams.cancelToken.Token);
            skillParams.cycleStartsStack.Push(stackItem);
            return stackItem.Item1;
        }
    }
}
