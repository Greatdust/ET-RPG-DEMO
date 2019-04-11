using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public struct Buff_HitEffect : IBuffData
{
    public string hitObjId;//击中时播放的特效
    public float duration ;//特效生命周期

    [NonSerialized]
    public GameObject effectGo;

    public string GetBuffIdType()
    {
        return BuffIdType.HitEffect;
    }

}
