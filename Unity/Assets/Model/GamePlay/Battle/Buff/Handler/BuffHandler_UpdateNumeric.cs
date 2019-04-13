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

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_UpdateNumeric buff = (Buff_UpdateNumeric)buffHandlerVar.data;

        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits bufferValue_TargetUnits))
        {
            Log.Error("找不到目标!");
            return;
        }

        NumericComponent numericComponent = buffHandlerVar.source.GetComponent<NumericComponent>();

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
        foreach (var v in bufferValue_TargetUnits.targets)
        {

            Game.EventSystem.Run(EventIdType.NumbericChange, buff.targetNumeric, v.Id, buff.updateValue);
        }
    }
    public void Remove(BuffHandlerVar buffHandlerVar)
    {
        Buff_UpdateNumeric buff = (Buff_UpdateNumeric)buffHandlerVar.data;

        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits bufferValue_TargetUnits))
        {
            Log.Error("找不到目标!");
            return;
        }
        foreach (var v in bufferValue_TargetUnits.targets)
        {
            Game.EventSystem.Run(EventIdType.NumbericChange, buff.targetNumeric, v.Id, -buff.updateValue);
        }
        buff.updateValue = 0;
    }
}



