using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
//这个一般不是技能用的,是道具,场景里的元素给予的直接回复效果
public struct Buff_GiveRecover : IBuffData
{
    public float hpValue;
    public float hpPct;
    public float mpValue;
    public float mpPct;

    public string GetBuffIdType()
    {
        return BuffIdType.GiveRecover;
    }
}
