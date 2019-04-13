using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.PlaySound)]
public class BuffHandler_PlaySound : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        Buff_PlaySound buff_PlaySound = (Buff_PlaySound)buffHandlerVar.data;

        AudioClip audioClip = Game.Scene.GetComponent<AudioCacheComponent>().Get(buff_PlaySound.audioClipName);



        AudioComponent audioComponent = buffHandlerVar.source.GetComponent<AudioComponent>();


        if (buff_PlaySound.onlyPlayOnceTime)
        {
            audioComponent.PlayAttackAudio(audioClip, buffHandlerVar.playSpeed, 0);

        }
        else
        {
            audioComponent.PlayAttackAudio(audioClip, buffHandlerVar.playSpeed, buff_PlaySound.duration);
        }
#endif
    }
}



