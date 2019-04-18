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

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        if (Game.Scene.GetComponent<GlobalConfigComponent>().networkPlayMode)
        {
            //联网模式是服务器发消息,才执行
            return;
        }
#endif
        Buff_DOT buff = (Buff_DOT)buffHandlerVar.data;
        NumericComponent numericComponent = buffHandlerVar.source.GetComponent<NumericComponent>();
        //这里只是计算一下DOT的伤害(快照机制),实际DOT的处理是在BuffMgr中管理的

        BuffHandlerVar.cacheDatas_int[(buffHandlerVar.source.Id,buff.buffSignal)] = Mathf.RoundToInt(numericComponent.GetAsFloat(buff.numericType) * buff.baseCoff * numericComponent.GetAsFloat(NumericType.FinalDamage_AddPct));

    }
}



