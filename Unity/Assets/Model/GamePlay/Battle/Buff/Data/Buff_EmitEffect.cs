using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 这是播放单个特效的BUFF,如果该BUFF的目标设置为多个了,那么会发射多次
/// </summary>
[Serializable]
public struct Buff_EmitEffect : IBuffData
{
    public string emitObjId;//发射时的特效
    public bool lockTarget;//是否是锁定目标的,如果是,发射方向就是计算使用者和第一个目标之前的方向
    public bool reverseDir;//下面所有的位置和方向设定,都是基于目标的位置和前方向的,即面对使用者的方向
    public Vector3 emitStartPos;//相对于使用者的位置,等于在使用者的位置和前方向上加上这个V3
    public Vector3 emitDir;//相对于使用者前方的方向,偏移了多少
    public float emitSpeed;//发射速度
    public float duration;//生命周期

    public List<string> effectParams ;

    public string GetBuffIdType()
    {
        return BuffIdType.EmitEffectInSkill;
    }

}
