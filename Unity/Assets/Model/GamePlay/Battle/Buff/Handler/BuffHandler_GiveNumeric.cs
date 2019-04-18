using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.GiveNumeric)]
public class BuffHandler_GiveNumeric : BaseBuffHandler, IBuffActionWithGetInputHandler
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
        Buff_GiveNumeric buff = (Buff_GiveNumeric)buffHandlerVar.data;
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }

        foreach (var v in targetUnits.targets)
        {
            Game.EventSystem.Run(EventIdType.NumbericChange, buff.numericType, v.Id, buff.value);

        }
    }
}



