using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.DOT)]
public class BuffHandler_DOT : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_DOT buff = data as Buff_DOT;
        NumericComponent numericComponent = source.GetComponent<NumericComponent>();
        //这里只是计算一下DOT的伤害(快照机制),实际DOT的处理是在BuffMgr中管理的
        buff.damageValue = Mathf.RoundToInt(numericComponent.GetAsFloat(buff.numericType) * buff.baseCoff * numericComponent.GetAsFloat(NumericType.FinalDamage_AddPct));
    }
}



