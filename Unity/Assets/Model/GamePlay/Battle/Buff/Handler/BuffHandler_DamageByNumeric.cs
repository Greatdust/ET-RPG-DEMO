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

    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_DamageByNumeric buff = data as Buff_DamageByNumeric;
        NumericComponent numericComponent = source.GetComponent<NumericComponent>();
        GameCalNumericTool.DamageData damageData = new GameCalNumericTool.DamageData();
        damageData.damageType = buff.damageType;
        damageData.damageValue = Mathf.RoundToInt(numericComponent.GetAsFloat(buff.numericType) * buff.baseCoff);
        damageData.isCritical = false;


        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;

            Game.EventSystem.Run(EventIdType.CalDamage, source.Id, target.Id, damageData);
        }
    }
}



