using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.CostHPMP)]
public class BuffHandler_CostHPMP : BaseBuffHandler, IBuffActionWithGetInputHandler
{
    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_CostHP_MP cost = data as Buff_CostHP_MP;

        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            NumericComponent numericComponent = target.GetComponent<NumericComponent>();
            float updateHp = -cost.costHp - cost.costHpInPct * numericComponent.GetAsFloat(NumericType.HPMax_Final);
            float updateMp = -cost.costMp - cost.costMpInPct * numericComponent.GetAsFloat(NumericType.MPMax_Final);
            if (updateHp != 0)
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, target.Id, updateHp);
            if (updateMp != 0)
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, target.Id, updateMp);
        }
    }
}




