using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.AddBuff)]
public class BuffHandler_AddBuff : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }

        Buff_AddBuff addBuff = (Buff_AddBuff)buffHandlerVar.data;
        addBuff.buffGroup.sourceUnitId = buffHandlerVar.source.Id;
        //添加BUFF时执行一下对应的Action
        foreach (var buff in addBuff.buffGroup.buffList)
        {

            BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(buff.GetBuffIdType());
            IBuffActionWithGetInputHandler buffActionWithGetInputHandler = baseBuffHandler as IBuffActionWithGetInputHandler;
            if (buffActionWithGetInputHandler != null)
            {
                BuffHandlerVar var1 = new BuffHandlerVar();
                if (buffHandlerVar.bufferValues != null)
                {
                    var1.bufferValues = new Dictionary<Type, IBufferValue>();

                    foreach (var v in buffHandlerVar.bufferValues)
                    {
                        var1.bufferValues.Add(v.Key, v.Value);
                    }
                }

                var1.source = buffHandlerVar.source;
                var1.skillLevel = buffHandlerVar.skillLevel;
                var1.playSpeed = 1;// 这个应该从角色属性计算得出,不过这里就先恒定为1好了.
                var1.data = buff;

                buffActionWithGetInputHandler.ActionHandle(var1);
            }
        }
        foreach (var v in targetUnits.targets)
        {
            BuffMgrComponent buffMgr = v.GetComponent<BuffMgrComponent>();
            buffMgr.AddBuffGroup(addBuff.buffGroup.BuffGroupId, addBuff.buffGroup);
        }
    }
}



