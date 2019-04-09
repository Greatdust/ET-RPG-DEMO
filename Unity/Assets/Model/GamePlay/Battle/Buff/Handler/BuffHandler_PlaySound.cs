using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.PlaySound)]
public class BuffHandler_PlaySound : BaseBuffHandler,IBuffActionWithGetInputHandler
{


    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        BuffReturnedValue_TargetUnit? target = null;
        foreach (var v in baseBuffReturnedValues)
        {
            target = v as BuffReturnedValue_TargetUnit?;
            if (target != null)
            {
                break;
            }
        }

        Buff_PlaySound buff_PlaySound = data as Buff_PlaySound;

        AudioClip audioClip = Game.Scene.GetComponent<AudioCacheComponent>().Get(buff_PlaySound.audioClipName);

        AudioComponent audioComponent = source.GetComponent<AudioComponent>();


        if (buff_PlaySound.onlyPlayOnceTime)
        {
            audioComponent.PlayAttackAudio(audioClip, target.Value.playSpeedScale, 0);

        }
        else
        {
            audioComponent.PlayAttackAudio(audioClip, target.Value.playSpeedScale, buff_PlaySound.duration);
        }
    }


}



