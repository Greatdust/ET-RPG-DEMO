using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.GiveNumeric)]
public class BuffHandler_GiveNumeric : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_GiveNumeric buff = data as Buff_GiveNumeric;

        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            Game.EventSystem.Run(EventIdType.NumbericChange, buff.numericType, target.Id, buff.value);

        }
    }
}



