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

    public int aimStackNum = 1;//叠加多少层之后才触发特殊效果

    [NonSerialized]
    public int currStackNum = 0;//当前叠加的层数

    public override string GetBuffIdType()
    {
        return BuffIdType.GiveSpecialDebuff;
    }
}
