using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SkillActiveCondition_CheckHPMP : BaseSkillData.IActiveConditionData
{
    public float costHp;
    public float costMp;
    public float costHpInPct;
    public float costMpInPct;

    public string GetBuffActiveConditionType()
    {
        return SkillActiveConditionType.CheckHPMP;
    }
}
