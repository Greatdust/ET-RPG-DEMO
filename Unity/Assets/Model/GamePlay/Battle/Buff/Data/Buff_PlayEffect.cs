using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 一般是Buff携带特效
/// </summary>
[LabelText("伴随特效")]
[LabelWidth(100)]
[Serializable]
public class Buff_PlayEffect : BaseBuffData
{
    [InfoBox("对应着在技能AB资源上的RC里定义的Id")]
    [LabelText("特效Id")]
    [LabelWidth(100)]
    public string effectObjId;//发射时的特效

    [LabelText("位置offset")]
    [LabelWidth(120)]
    public Vector3 posOffset;//相对于使用者的位置,等于在使用者的位置和前方向上加上这个V3


    public List<string> effectParams ;

    public override string GetBuffIdType()
    {
        return BuffIdType.PlayEffect;
    }

}
