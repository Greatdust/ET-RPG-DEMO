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
            case NumericRealtionType.Greater:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) > checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.Less:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) < checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.Equal:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) == checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.NotEqual:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) != checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.GreaterEqual:
                if (numericComponent.GetAsFloat(checkNumeric.numericType) >= checkNumeric.aimValue)
                    return true;
                break;
            case NumericRealtionType.LessEqual:
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
