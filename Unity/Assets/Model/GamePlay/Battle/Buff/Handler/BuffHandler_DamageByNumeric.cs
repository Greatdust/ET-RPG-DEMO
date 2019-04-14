using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.DamageByNumeric)]
public class BuffHandler_DamageByNumeric : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_DamageByNumeric buff = (Buff_DamageByNumeric)buffHandlerVar.data;
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }
        NumericComponent numericComponent = buffHandlerVar.source.GetComponent<NumericComponent>();
        GameCalNumericTool.DamageData damageData = new GameCalNumericTool.DamageData();

        

        damageData.damageType = buff.damageType;

        damageData.damageValue = Mathf.RoundToInt(numericComponent.GetAsFloat(buff.numericType) * buff.baseCoff);
        if (buffHandlerVar.bufferValues.TryGetValue(typeof(BufferValue_DamageAddPct),out var damageAddPct))
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


        foreach (var v in targetUnits.targets)
        {

            Game.EventSystem.Run(EventIdType.CalDamage, buffHandlerVar.source.Id, v.Id, damageData);
        }
    }
}



