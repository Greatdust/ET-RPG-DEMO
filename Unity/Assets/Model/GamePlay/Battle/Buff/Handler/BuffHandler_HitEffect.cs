using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PF;
using UnityEngine;

[BuffType(BuffIdType.HitEffect)]
public class BuffHandler_HitEffect : BaseBuffHandler,IBuffActionWithGetInputHandler
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_HitEffect buff = (Buff_HitEffect)buffHandlerVar.data;
        if (buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targets))
        {

            foreach (var v in targets.targets)
            {
                //未造成伤害就不给予效果
                if (buffHandlerVar.GetBufferValue(out BufferValue_AttackSuccess attackSuccess))
                {
                    if (attackSuccess.successDic.ContainsKey(v.Id))
                        if (!attackSuccess.successDic[v.Id]) continue;
                }
                HitEffect(buff, v.Position, buffHandlerVar.skillId);
            }
            return;
        }
        if (!buffHandlerVar.GetBufferValue(out BufferValue_Pos value_Pos))
        {
            return;
        }
        HitEffect(buff, value_Pos.aimPos, buffHandlerVar.skillId);


    }

    public void HitEffect(Buff_HitEffect buff, Vector3 pos,string skillId)
    {
#if !SERVER
        PlayHitEffect(buff.hitObjId, pos + buff.posOffset.ToV3(), buff.duration).Coroutine();
#else
        bool isInApplyData = false;
        BaseSkillData baseSkillData = SkillHelper.GetBaseSkillData(skillId);
        foreach (var v in baseSkillData.applyDatas)
        {
            PipelineDataWithBuff pipelineDataWithBuff = v as PipelineDataWithBuff;
            if (pipelineDataWithBuff != null)
            {
                if (pipelineDataWithBuff.buffs.Find(b => b.buffData.buffSignal == buff.buffSignal)!=null)
                {
                    isInApplyData = true;
                    break;
                }
            }
        }
        if (!isInApplyData) return;
        M2C_HitEffect m2C = new M2C_HitEffect();
        m2C.Duration = buff.duration;
        m2C.HitObjId = buff.hitObjId;
        m2C.Pos = (pos + buff.posOffset.ToV3()).ToV3Info();
        ETHotfix.MessageHelper.Broadcast(m2C);
#endif
    }

#if !SERVER
    public async static ETVoid PlayHitEffect(string hitObjId, Vector3 pos, float duration)
    {
        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(hitObjId);//先找到缓存的特效物体
        var effectGo = go;
        go.SetActive(false);

        //在目标位置处播放
        go.transform.position = pos;

        go.SetActive(true);
        await TimerComponent.Instance.WaitAsync(duration);
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(hitObjId, effectGo);
    }
#endif

}



