using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[SkillActiveCondition(SkillActiveConditionType.CheckEquip)]
public class SkillActiveConditionHandler_CheckEquip : BaseSkillData.IActiveConditionHandler
{


    public bool MeetCondition(BaseSkillData.IActiveConditionData data, Unit source)
    {
        SkillActiveCondition_CheckEquip CheckEquip = data as SkillActiveCondition_CheckEquip;
        EquipmentComponent equipmentComponent = source.GetComponent<EquipmentComponent>();
        if (CheckEquip.CheckWeapon)
        {
            WeaponData weaponData = equipmentComponent.GetEquipData(EquipPosType.武器) as WeaponData;
            if (weaponData != null)
            {
                if (weaponData.weaponType == CheckEquip.targetWeapon)
                {
                    return true;
                }
            }
        }

        return false;
    }


    public void OnRemove(BaseSkillData.IActiveConditionData data, Unit source)
    {
      
    }
}
