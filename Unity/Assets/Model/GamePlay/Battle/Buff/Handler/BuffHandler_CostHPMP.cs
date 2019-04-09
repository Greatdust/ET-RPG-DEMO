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
    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        Buff_CostHP_MP cost = data as Buff_CostHP_MP;

        foreach (var v in baseBuffReturnedValues)
        {
            BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = v as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            NumericComponent numericComponent = target.GetComponent<NumericComponent>();
            float updateHp = -cost.costHp - cost.costHpInPct * numericComponent.GetAsFloat(NumericType.气血MaxFinal);
            float updateMp = -cost.costMp - cost.costMpInPct * numericComponent.GetAsFloat(NumericType.法力MaxFinal);
            if (updateHp != 0)
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, target.Id, updateHp);
            if (updateMp != 0)
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, target.Id, updateMp);
        }
    }
}




