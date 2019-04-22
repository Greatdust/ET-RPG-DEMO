using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[SkillActiveCondition(SkillActiveConditionType.CheckBuff)]
public class SkillActiveConditionHandler_CheckBuff : BaseSkillData.IActiveConditionHandler
{


    public bool MeetCondition(BaseSkillData.IActiveConditionData data, Unit source)
    {
        SkillActiveCondition_CheckBuff CheckBuff = data as SkillActiveCondition_CheckBuff;
        BuffMgrComponent buffMgrComponent = source.GetComponent<BuffMgrComponent>();
        foreach (var v in buffMgrComponent.buffGroupDic)
        {
            if (v.Value.buffTypeId == CheckBuff.buffTypeId)
            {
                return true;
            }
        }

        return false;
    }

    public void OnRemove(BaseSkillData.IActiveConditionData data, Unit source)
    {
     
    }
}
