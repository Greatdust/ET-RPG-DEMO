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

        Buff_PlaySound buff_PlaySound = (Buff_PlaySound)buffHandlerVar.data;
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }

        foreach (var v in targetUnits.targets)
        {
            if (v.UnitData.unitLayer == UnitLayer.Default)
            {
                continue;
            }
            if (buffHandlerVar.GetBufferValue(out BufferValue_AttackSuccess attackSuccess))
            {
                if (attackSuccess.successDic.ContainsKey(v.Id))
                    if (!attackSuccess.successDic[v.Id]) continue;
            }
            PlayAudio(buff_PlaySound, v, buffHandlerVar.playSpeed,buffHandlerVar.skillId);
        }

    }

    public void PlayAudio(Buff_PlaySound buff_PlaySound, Unit v, float playSpeed,string skillId)
    {
#if !SERVER
        PlayAudio(buff_PlaySound.audioClipName, v, playSpeed, buff_PlaySound.onlyPlayOnceTime ? 0 : buff_PlaySound.duration);
#else
        bool isInApplyData = false;
        BaseSkillData baseSkillData = SkillHelper.GetBaseSkillData(skillId);
        foreach (var _v in baseSkillData.applyDatas)
        {
            PipelineDataWithBuff pipelineDataWithBuff = _v as PipelineDataWithBuff;
            if (pipelineDataWithBuff != null)
            {
                if (pipelineDataWithBuff.buffs.Find(b => b.buffData.buffSignal == buff_PlaySound.buffSignal) != null)
                {
                    isInApplyData = true;
                    break;
                }
            }
        }
        if (!isInApplyData) return;
        M2C_PlaySound m2C = new M2C_PlaySound();
        m2C.Duration = buff_PlaySound.onlyPlayOnceTime ? 0 : buff_PlaySound.duration;
        m2C.AudioClipName = buff_PlaySound.audioClipName;
        m2C.Id = v.Id;
        m2C.PlaySpeed = playSpeed;
        ETHotfix.MessageHelper.Broadcast(m2C);
#endif
    }
    #if !SERVER
    public static void PlayAudio(string audioClipName, Unit v, float playSpeed, float duration)
    {
        AudioComponent audioComponent = v.GetComponent<AudioComponent>();

        AudioClip audioClip = Game.Scene.GetComponent<AudioCacheComponent>().Get(audioClipName);
        if (duration > 0.01f)
        {
            audioComponent.PlayAttackSound(audioClip, playSpeed, 0);

        }
        else
        {
            audioComponent.PlayAttackSound(audioClip, playSpeed, duration);
        }
    }
#endif
}



