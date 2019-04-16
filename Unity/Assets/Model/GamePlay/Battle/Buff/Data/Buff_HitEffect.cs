using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[LabelText("击中特效")]
[LabelWidth(150)]
[Serializable]
public class Buff_HitEffect : BaseBuffData
{
    public string hitObjId;//击中时播放的特效
    public float duration ;//特效生命周期
    public Vector3Serializer posOffset;

    public override string GetBuffIdType()
    {
        return BuffIdType.HitEffect;
    }

}
