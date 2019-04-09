using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SkillActiveCondition_CheckTiming : BaseSkillData.IActiveConditionData
{
    public float timeSpan;//间隔多少时间执行一次

    public string GetBuffActiveConditionType()
    {
        return SkillActiveConditionType.CheckTiming;
    }
}
