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
        if (timer.remainTime == 0 && timer.timing ==0)
        {
            timer.remainTime = CheckTiming.timeSpan;
        }
        float delta = Time.deltaTime;
        timer.timing += delta;
        timer.remainTime -= delta;
        if (timer.remainTime <= 0)
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
