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

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_GiveNumeric buff = (Buff_GiveNumeric)buffHandlerVar.data;
        BufferValue_TargetUnits buffReturnedValue_TargetUnit = (BufferValue_TargetUnits)buffHandlerVar.bufferValues[typeof(BufferValue_TargetUnits)];
        foreach (var v in buffReturnedValue_TargetUnit.targets)
        {
            Game.EventSystem.Run(EventIdType.NumbericChange, buff.numericType, v.Id, buff.value);

        }
    }
}



