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

    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        Buff_DOT buff = data as Buff_DOT;
        NumericComponent numericComponent = source.GetComponent<NumericComponent>();
        buff.damageFinalAddPct = numericComponent.GetAsFloat(NumericType.最终伤害加成率);
        switch (buff.damageType)
        {
            case GameCalNumericTool.DamageType.无属性伤害:
                break;
            case GameCalNumericTool.DamageType.金属性伤害:
                buff.qinHe = numericComponent.GetAsInt(NumericType.金属性亲和);
                break;
            case GameCalNumericTool.DamageType.木属性伤害:
                buff.qinHe = numericComponent.GetAsInt(NumericType.木属性亲和);
                break;
            case GameCalNumericTool.DamageType.水属性伤害:
                buff.qinHe = numericComponent.GetAsInt(NumericType.水属性亲和);
                break;
            case GameCalNumericTool.DamageType.火属性伤害:
                buff.qinHe = numericComponent.GetAsInt(NumericType.火属性亲和);
                break;
            case GameCalNumericTool.DamageType.土属性伤害:
                buff.qinHe = numericComponent.GetAsInt(NumericType.土属性亲和);
                break;
        }
        //这里只是计算一下DOT的伤害(快照机制),实际DOT的处理是在BuffMgr中管理的
        buff.damageValue = Mathf.RoundToInt(numericComponent.GetAsFloat(buff.numericType) * buff.coefficient);
    }
}



