using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[SkillActiveCondition(SkillActiveConditionType.CheckNumeric)]
public class SkillActiveConditionHandler_CheckNumeric : BaseSkillData.IActiveConditionHandler
{


    public bool MeetCondition(BaseSkillData.IActiveConditionData data, Unit source)
    {
        SkillActiveCondition_CheckNumeric checkNumeric = data as SkillActiveCondition_CheckNumeric;
        NumericComponent numericComponent = source.GetComponent<NumericComponent>();
        switch (checkNumeric.realtionType)
        {
            case NumericRealtionType.大于:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) > checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.小于:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) < checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.等于:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) == checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.不等于:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) != checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.大于等于:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) >= checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.小于等于:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) <= checkNumeric.aimValue)
                    return true;
                break;
        }
        return false;
    }


    public void OnRemove(BaseSkillData.IActiveConditionData data, Unit source)
    {
        
    }
}
