using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[SkillActiveCondition(SkillActiveConditionType.CheckTiming)]
public class SkillActiveConditionHandler_CheckTiming : BaseSkillData.IActiveConditionHandler
{


    public bool MeetCondition(BaseSkillData.IActiveConditionData data, Unit source)
    {
        SkillActiveCondition_CheckTiming CheckTiming = data as SkillActiveCondition_CheckTiming;
        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(CheckTiming.GetHashCode());
        long now = TimeHelper.Now();
        if (timer.interval == 0 && timer.timing ==0)
        {
            timer.interval =(long)(CheckTiming.timeSpan*1000);
            timer.timing = now;
        }
        if (now - timer.timing >= timer.interval)
        {
            TimeSpanHelper.Remove(CheckTiming.GetHashCode());
            return true;
        }
        return false;
    }


    public void OnRemove(BaseSkillData.IActiveConditionData data, Unit source)
    {
        
    }
}
