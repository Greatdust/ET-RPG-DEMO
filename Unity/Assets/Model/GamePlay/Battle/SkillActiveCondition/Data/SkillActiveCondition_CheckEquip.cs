using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class SkillActiveCondition_CheckEquip : BaseSkillData.IActiveConditionData
{
    public bool CheckWeapon;
    public WeaponType targetWeapon;

    public string GetBuffActiveConditionType()
    {
        return SkillActiveConditionType.CheckEquip;
    }
}
