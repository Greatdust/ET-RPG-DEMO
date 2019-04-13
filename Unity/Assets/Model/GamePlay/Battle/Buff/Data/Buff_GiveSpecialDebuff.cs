using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class Buff_GiveSpecialDebuff : BaseBuffData
{
    public RestrictionType restrictionType;

    public int aimStackNum ;//叠加多少层之后才触发特殊效果

    public override string GetBuffIdType()
    {
        return BuffIdType.GiveSpecialDebuff;
    }
}
