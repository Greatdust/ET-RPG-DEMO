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

    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        Buff_DamageByNumeric buff = data as Buff_DamageByNumeric;
        NumericComponent numericComponent = source.GetComponent<NumericComponent>();
        GameCalNumericTool.DamageData damageData = new GameCalNumericTool.DamageData();
        damageData.damageType = buff.damageType;
        damageData.damageValue = Mathf.RoundToInt(numericComponent.GetAsFloat(buff.numericType) * buff.coefficient);
        damageData.isCritical = false;

        foreach (var v in baseBuffReturnedValues)
        {
            BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = v as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            Game.EventSystem.Run(EventIdType.CalDamage, source.Id, target.Id, damageData);
        }
    }
}



