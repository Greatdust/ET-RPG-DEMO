using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PF;
using UnityEngine;

[BuffType(BuffIdType.PlayEffect)]
public class BuffHandler_PlayEffect : BaseBuffHandler, IBuffActionWithGetInputHandler, IBuffRemoveHanlder
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        Buff_PlayEffect buff = (Buff_PlayEffect)buffHandlerVar.data;
        if (buff.lockToTarget)
        {
            if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
            {

                return;
            }
            foreach (var v in targetUnits.targets)
            {
                PlayEffect_LockToTarget(v, buff, buffHandlerVar);
            }
        }
        else
        {
            if (!buffHandlerVar.GetBufferValue(out BufferValue_Pos pos))
            {
                Log.Debug("找不到位置");
                return;
            }
            PlayEffect(pos.aimPos, buff, buffHandlerVar);
        }
#endif
    }
    public void PlayEffect_LockToTarget(Unit target, Buff_PlayEffect buff, BuffHandlerVar buffHandlerVar)
    {
        AddEffect(target, buff.buffSignal, buff.effectObjId, true, target.Position + buff.posOffset.ToV3(), buff.canBeInterrupted, buffHandlerVar.cancelToken, buff.duration,buffHandlerVar.skillId).Coroutine();
    }

    public void PlayEffect(Vector3 target, Buff_PlayEffect buff, BuffHandlerVar buffHandlerVar)
    {
        AddEffect(buffHandlerVar.source, buff.buffSignal, buff.effectObjId, false, target + buff.posOffset.ToV3(), buff.canBeInterrupted, buffHandlerVar.cancelToken, buff.duration, buffHandlerVar.skillId).Coroutine();
    }


    public void Remove(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        Buff_PlayEffect buff = (Buff_PlayEffect)buffHandlerVar.data;
        if (buff.duration > 0)
        {
            return;
        }
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }
        foreach (var v in targetUnits.targets)
        {
            RemoveEffect(v.Id, buff.buffSignal, buff.effectObjId);
        }
#endif
    }

    public static async ETVoid AddEffect(Unit unit, string buffSignal, string effectObjId, bool lockTarget, Vector3 target, bool canBeInterrupt, CancellationToken cancellationToken, float duration,string skillId)
    {
#if !SERVER
        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(effectObjId);//先找到缓存的特效物体

        BuffHandlerVar.cacheDatas_object[(unit.Id, buffSignal)] = go;

        go.SetActive(false);

        //在目标位置处播放,并跟随
        go.transform.position = target;
        if (lockTarget)
        {
            go.transform.parent = unit.GameObject.transform;
        }

        go.SetActive(true);
        if (canBeInterrupt)
            cancellationToken.Register(() =>
            {
                go.transform.parent = null;
                Game.Scene.GetComponent<EffectCacheComponent>().Recycle(effectObjId, go);
            });
        if (duration > 0)
        {
            if (canBeInterrupt)
                await TimerComponent.Instance.WaitAsync((long)(duration * 1000), cancellationToken);
            else
                await TimerComponent.Instance.WaitAsync(duration);
        }
        go.transform.parent = null;
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(effectObjId, go);
#else
        bool isInApplyData = false;
        BaseSkillData baseSkillData = SkillHelper.GetBaseSkillData(skillId);
        foreach (var v in baseSkillData.applyDatas)
        {
            PipelineDataWithBuff pipelineDataWithBuff = v as PipelineDataWithBuff;
            if (pipelineDataWithBuff != null)
            {
                if (pipelineDataWithBuff.buffs.Find(b => b.buffData.buffSignal == buffSignal) != null)
                {
                    isInApplyData = true;
                    break;
                }
            }
        }
        if (!isInApplyData) return;
        M2C_PlayEffect m2C = new M2C_PlayEffect();
        m2C.Duration = duration;
        m2C.BuffSignal = buffSignal;
        m2C.CanBeInterupt = canBeInterrupt;
        m2C.EffectObjId = effectObjId;
        m2C.LockTarget = lockTarget;
        m2C.Id = unit.Id;
        m2C.Pos = target.ToV3Info();
        ETHotfix.MessageHelper.Broadcast(m2C);
#endif
    }
#if !SERVER
    public static void RemoveEffect(long unitId, string buffSignal, string effectObjId)
    {
        GameObject go;

        go = BuffHandlerVar.cacheDatas_object[(unitId, buffSignal)] as UnityEngine.GameObject;
        if (go != null)
        {
            go.transform.parent = null;
            Game.Scene.GetComponent<EffectCacheComponent>().Recycle(effectObjId, go);
        }
    }
#endif

}



