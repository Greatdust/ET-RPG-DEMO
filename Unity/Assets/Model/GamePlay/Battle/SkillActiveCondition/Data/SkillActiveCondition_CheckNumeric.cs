using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SkillActiveCondition_CheckNumeric : BaseSkillData.IActiveConditionData
{
    public NumericType numericType;
    public NumericRealtionType realtionType;
    public float aimValue;

    public string GetBuffActiveConditionType()
    {
        return SkillActiveConditionType.CheckNumeric;
    }
}
