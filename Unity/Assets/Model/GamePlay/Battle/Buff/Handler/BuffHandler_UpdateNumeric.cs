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

            BuffHandlerVar.cacheDatas_float[(buffHandlerVar.source.Id, buff.buffSignal)] = value;
        }
        else
        {
            BuffHandlerVar.cacheDatas_float[(buffHandlerVar.source.Id, buff.buffSignal)] = buff.valueAdd;
        }
        foreach (var v in bufferValue_TargetUnits.targets)
        {

            Game.EventSystem.Run(EventIdType.NumbericChange, buff.targetNumeric, v.Id,(float)BuffHandlerVar.cacheDatas_float[(buffHandlerVar.source.Id, buff.buffSignal)]);
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
            Game.EventSystem.Run(EventIdType.NumbericChange, buff.targetNumeric, v.Id, -(float)BuffHandlerVar.cacheDatas_float[(buffHandlerVar.source.Id, buff.buffSignal)]);
        }
        BuffHandlerVar.cacheDatas_float.Remove((buffHandlerVar.source.Id, buff.buffSignal));
    }
}



