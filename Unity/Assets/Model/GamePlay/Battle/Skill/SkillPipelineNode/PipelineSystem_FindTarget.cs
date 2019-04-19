using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class PipelineSystem_FindTarget
{
    public static void Execute(this Pipeline_FindTarget findTarget, SkillHelper.ExecuteSkillParams skillParams)
    {
        switch (findTarget.findTargetType)
        {
            case FindTargetType.自身:
                SkillHelper.tempData[(skillParams.source, findTarget.pipelineSignal)] = new Dictionary<Type, IBufferValue>();
                SkillHelper.tempData[(skillParams.source, findTarget.pipelineSignal)][typeof(BufferValue_TargetUnits)] = new BufferValue_TargetUnits() { targets = new Unit[] { skillParams.source } };
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
    }
}
