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


    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        BufferValue_TargetUnits? target = null;
        foreach (var v in baseBuffReturnedValues)
        {
            target = v as BufferValue_TargetUnits?;
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



