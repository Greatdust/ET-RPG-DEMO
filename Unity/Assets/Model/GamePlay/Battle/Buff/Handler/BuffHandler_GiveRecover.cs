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

    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        Buff_GiveRecover buff = data as Buff_GiveRecover;
        foreach (var v in baseBuffReturnedValues)
        {
            BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = v as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            NumericComponent numericComponent = target.GetComponent<NumericComponent>();
            if (buff.hpValue > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP_Final, target.Id, buff.hpValue);
            }
            if (buff.hpPct > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP_Final, target.Id, numericComponent.GetAsFloat(NumericType.HPMax_Final) * buff.hpPct);
            }
            if (buff.mpValue > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, target.Id, buff.mpValue);
            }
            if (buff.mpPct > 0)
            {
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, target.Id, numericComponent.GetAsFloat(NumericType.MPMax_Final) * buff.mpPct);
            }

        }
    }
}



