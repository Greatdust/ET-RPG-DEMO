using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
[ObjectSystem]
public class AudioMgrComponentAwakeSystem : AwakeSystem<AudioMgrComponent, AudioPlayerMgr>
{
    public override void Awake(AudioMgrComponent self, AudioPlayerMgr audioPlayerMgr)
    {
        self.Awake(audioPlayerMgr);
    }
}

public class AudioMgrComponent : ETModel.Component
{
    public AudioPlayerMgr audioPlayerMgr;

    public void Awake(AudioPlayerMgr audioPlayerMgr)
    {
        this.audioPlayerMgr = audioPlayerMgr;
    }

    public void PlayBg(AudioClip audioClip)
    {
        audioPlayerMgr.PlayBg(audioClip);
    }

    public void PauseBg()
    {
        audioPlayerMgr.PauseBg();
    }

    
}

