using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SkillActiveCondition_CheckBuff : BaseSkillData.IActiveConditionData
{
    public string buffName;

    public string GetBuffActiveConditionType()
    {
        return SkillActiveConditionType.CheckBuff;
    }
}
