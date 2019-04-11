using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.UpdateNumeric)]
public class BuffHandler_UpdateNumeric : BaseBuffHandler, IBuffActionWithGetInputHandler, IBuffRemoveHanlder
{


    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_UpdateNumeric buff = data as Buff_UpdateNumeric;
        NumericComponent numericComponent = source.GetComponent<NumericComponent>();

        if (buff.addValueByNumeric)
        {
            float value = numericComponent.GetAsFloat(buff.sourceNumeric) * buff.coefficient + buff.valueAdd;
            if (buff.updateValue == value) return;
            buff.updateValue = value;
        }
        else
        {
            if (buff.updateValue == buff.valueAdd) return;
            buff.updateValue = buff.valueAdd;
        }
        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            Game.EventSystem.Run(EventIdType.NumbericChange, buff.targetNumeric, target.Id, buff.updateValue);
        }
    }

    public void Remove(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_UpdateNumeric buff = data as Buff_UpdateNumeric;
       
        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            Game.EventSystem.Run(EventIdType.NumbericChange, buff.targetNumeric, target.Id, -buff.updateValue);
        }
        buff.updateValue = 0;
    }
}



