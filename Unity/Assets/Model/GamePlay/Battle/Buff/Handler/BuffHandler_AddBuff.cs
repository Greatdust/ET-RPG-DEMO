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


    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        Buff_AddBuff buff = data as Buff_AddBuff;
        if (buff.buffGroup == null) return;
        buff.buffGroup.sourceUnitId = source.Id;
        foreach (var v in baseBuffReturnedValues)
        {
            BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = v as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            BuffMgrComponent buffMgr = target.GetComponent<BuffMgrComponent>();
            buffMgr.AddBuffGroup(buff.buffGroup.BuffGroupId, buff.buffGroup);
        }
    }
}



