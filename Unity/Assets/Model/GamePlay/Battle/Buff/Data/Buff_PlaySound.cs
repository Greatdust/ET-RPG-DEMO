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
public struct Buff_PlaySound : IBuffData
{
    [LabelText("音频名")]
    [InfoBox("这个音频名是技能资源AB包上的RC组件里定义的Key")]
    [LabelWidth(150)]
    public string audioClipName;//要播放的声音音频
    [LabelText("只播放一次")]
    [LabelWidth(150)]
    public bool onlyPlayOnceTime;//只播放一次就消失
    [HideIf("onlyPlayOnceTime")]
    public float duration;//这个时候声音会循环播放,生命周期到达时,会删除

    public string GetBuffIdType()
    {
        return BuffIdType.PlaySound;
    }
}
