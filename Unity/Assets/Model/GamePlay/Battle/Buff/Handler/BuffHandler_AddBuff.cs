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
        foreach (var v in targetUnits.targets)
        {
            //未造成伤害就不给予效果
            if (buffHandlerVar.GetBufferValue(out BufferValue_AttackSuccess attackSuccess))
            {
                if(attackSuccess.successDic.ContainsKey(v.Id))
                if (!attackSuccess.successDic[v.Id]) continue;
            }
            BuffMgrComponent buffMgr = v.GetComponent<BuffMgrComponent>();
            buffMgr.AddBuffGroup(addBuff.buffGroup.BuffGroupId, addBuff.buffGroup);
        }
    }
}



