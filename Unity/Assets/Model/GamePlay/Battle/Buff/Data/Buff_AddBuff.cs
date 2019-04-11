using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[Serializable]
public struct Buff_AddBuff : IBuffData
{
    public BuffGroup buffGroup;

    public string GetBuffIdType()
    {
        return BuffIdType.AddBuff;
    }

}
