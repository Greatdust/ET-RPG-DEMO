using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.DirectDamage)]
public class BuffHandler_DirectDamage : BaseBuffHandler, IBuffActionWithGetInputHandler
{


    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_DirectDamage buff = data as Buff_DirectDamage;
        GameCalNumericTool.DamageData damageData = new GameCalNumericTool.DamageData();
        damageData.damageType = buff.damageType;
        damageData.damageValue = buff.damageValue;
        damageData.isCritical = false;
        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            Game.EventSystem.Run(EventIdType.CalDamage, source.Id, target.Id, damageData);

        }
    }
}



