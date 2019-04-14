using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.GiveRecover)]
public class BuffHandler_GiveRecover : BaseBuffHandler,IBuffActionWithGetInputHandler
{


    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_GiveRecover buff = (Buff_GiveRecover)buffHandlerVar.data;

        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }

        foreach (var v in targetUnits.targets)
        {
            NumericComponent numericComponent = v.GetComponent<NumericComponent>();
            if (buff.hpValue > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, v.Id, buff.hpValue);
            }
            if (buff.hpPct > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, v.Id, numericComponent.GetAsFloat(NumericType.HPMax_Final) * buff.hpPct);
            }
            if (buff.mpValue > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, v.Id, buff.mpValue);
            }
            if (buff.mpPct > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, v.Id, numericComponent.GetAsFloat(NumericType.MPMax_Final) * buff.mpPct);
            }
        }
    }
}



