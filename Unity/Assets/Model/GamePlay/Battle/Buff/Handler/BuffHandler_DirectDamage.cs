using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.DirectDamage)]
public class BuffHandler_DirectDamage : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_DirectDamage buff = (Buff_DirectDamage)buffHandlerVar.data;
        BufferValue_TargetUnits buffReturnedValue_TargetUnit = (BufferValue_TargetUnits)buffHandlerVar.bufferValues[typeof(BufferValue_TargetUnits)];
        GameCalNumericTool.DamageData damageData = new GameCalNumericTool.DamageData();


        damageData.damageType = buff.damageType;
        damageData.damageValue = buff.damageValue;
        if (buffHandlerVar.bufferValues.TryGetValue(typeof(BufferValue_DamageAddPct), out var damageAddPct))
        {
            damageData.damageValue = Mathf.RoundToInt((1 + ((BufferValue_DamageAddPct)damageAddPct).damageAddPct) * damageData.damageValue);
        }
        damageData.isCritical = false;
        SkillEffectComponent skillEffectComponent = buffHandlerVar.source.GetComponent<SkillEffectComponent>();
        var effectData = skillEffectComponent.GetEffectData(buffHandlerVar.skillId);
        if (effectData != null)
        {
            damageData.damageValue = Mathf.RoundToInt((1 + effectData.coefficientAddPct) * damageData.damageValue);
            damageData.isCritical = effectData.critical;
        }

        foreach (var v in buffReturnedValue_TargetUnit.targets)
        {
            Game.EventSystem.Run(EventIdType.CalDamage, buffHandlerVar.source.Id, v.Id, damageData);
        }
    }
}



