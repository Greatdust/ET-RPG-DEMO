using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public struct Buff_GiveSpecialDebuff : IBuffData
{
    public RestrictionType restrictionType;

    public int aimStackNum ;//叠加多少层之后才触发特殊效果

    [NonSerialized]
    public int currStackNum ;//当前叠加的层数

    public string GetBuffIdType()
    {
        return BuffIdType.GiveSpecialDebuff;
    }
}
