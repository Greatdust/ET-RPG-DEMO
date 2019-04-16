using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine;
[ObjectSystem]
public class AudioComponentAwakeSystem : AwakeSystem<AudioComponent>
{
    public override void Awake(AudioComponent self)
    {
        self.Awake();
    }
}

public class AudioComponent : ETModel.Component
{

    public UnitAudioSourceHelper unitAudioSourceHelper;


    public void Awake()
    {
        unitAudioSourceHelper = GetParent<Unit>().GameObject.GetComponentInChildren<UnitAudioSourceHelper>();
        AudioPlayerMgr audioPlayerMgr = Game.Scene.GetComponent<AudioMgrComponent>().audioPlayerMgr;
        if (unitAudioSourceHelper.attackAS != null)
            unitAudioSourceHelper.attackAS.outputAudioMixerGroup = audioPlayerMgr.soundEffect;
        if (unitAudioSourceHelper.envirAS != null)
            unitAudioSourceHelper.envirAS.outputAudioMixerGroup = audioPlayerMgr.soundEffect;
        if (unitAudioSourceHelper.emoteAS != null)
            unitAudioSourceHelper.emoteAS.outputAudioMixerGroup = audioPlayerMgr.characterSound;
        if (unitAudioSourceHelper.moveAS != null)
            unitAudioSourceHelper.moveAS.outputAudioMixerGroup = audioPlayerMgr.characterSound;
    }


    public void PlayAttackSound(AudioClip audioClip, float pitch = 1, float duration = 0)
    {
        if (unitAudioSourceHelper.attackAS != null)
        {
            //unitAudioSourceHelper.attackAS.pitch = pitch;
            PlayAudio(unitAudioSourceHelper.attackAS, audioClip, duration).Coroutine();
        }
    }

    public void PlayMoveSound(float pitch)
    {
        if (unitAudioSourceHelper.moveAS != null)
        {
            unitAudioSourceHelper.PlayMoveSound(pitch);
        }
    }

    public void PauseMoveSound()
    {
        if (unitAudioSourceHelper.moveAS != null)
        {
            unitAudioSourceHelper.PauseMoveSound();
        }
    }


    async ETVoid PlayAudio(AudioSource audioSource, AudioClip clip, float duration = 0)
    {
        if (duration < 0.01f)
        {
            audioSource.loop = false;
            audioSource.PlayOneShot(clip);
        }
        else
        {
            audioSource.loop = true;
            audioSource.clip = clip;
            audioSource.Play();
            await TimerComponent.Instance.WaitAsync(duration);
            audioSource.Stop();
            audioSource.clip = null;
        }
    }

  
    
}

