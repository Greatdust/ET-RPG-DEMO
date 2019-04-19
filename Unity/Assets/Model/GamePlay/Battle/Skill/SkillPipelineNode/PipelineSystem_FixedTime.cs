using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class PipelineSystem_FixedTime
{
    public static async ETTask  Execute(this Pipeline_FixedTime pipeline_fixedTime, SkillHelper.ExecuteSkillParams skillParams,BuffHandlerVar buffHandlerVar)
    {
        if (pipeline_fixedTime.delayTime > 0)
        {
            await TimerComponent.Instance.WaitAsync((long)(pipeline_fixedTime.delayTime * skillParams.playSpeed * 1000), skillParams.cancelToken.Token);

        }

        SkillHelper.HandleBuffs(pipeline_fixedTime, buffHandlerVar, skillParams).Coroutine();

        if (pipeline_fixedTime.fixedTime > 0)
        {
            await TimerComponent.Instance.WaitAsync((long)(pipeline_fixedTime.fixedTime * skillParams.playSpeed * 1000), skillParams.cancelToken.Token);
        }
    }
}
