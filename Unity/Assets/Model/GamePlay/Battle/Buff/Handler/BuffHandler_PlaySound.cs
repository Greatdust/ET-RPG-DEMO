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
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }
        AudioClip audioClip = Game.Scene.GetComponent<AudioCacheComponent>().Get(buff_PlaySound.audioClipName);
        foreach (var v in targetUnits.targets)
        {
         
            if (buffHandlerVar.GetBufferValue(out BufferValue_AttackSuccess attackSuccess))
            {
                if (!attackSuccess.successDic[v.Id]) continue;
            }
            AudioComponent audioComponent = v.GetComponent<AudioComponent>();


            if (buff_PlaySound.onlyPlayOnceTime)
            {
                audioComponent.PlayAttackSound(audioClip, buffHandlerVar.playSpeed, 0);

            }
            else
            {
                audioComponent.PlayAttackSound(audioClip, buffHandlerVar.playSpeed, buff_PlaySound.duration);
            }
        }
#endif
    }
}



