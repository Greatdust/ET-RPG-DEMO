using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[LabelText("播放音频")]
[LabelWidth(150)]
[Serializable]
public class Buff_PlaySound : BaseBuffData
{
    public string audioClipName;//要播放的声音音频
    public bool onlyPlayOnceTime;//只播放一次就消失
    public float duration;//这个时候声音会循环播放,生命周期到达时,会删除

    public override string GetBuffIdType()
    {
        return BuffIdType.PlaySound;
    }
}
