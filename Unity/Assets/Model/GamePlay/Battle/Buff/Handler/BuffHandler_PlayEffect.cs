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
public class BuffHandler_PlayEffect : BaseBuffHandler,IBuffActionWithGetInputHandler,IBuffRemoveHanlder
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
                PlayEffect_LockToTarget(v, buff, buffHandlerVar).Coroutine();
            }
        }
        else
        {
            if (!buffHandlerVar.GetBufferValue(out BufferValue_Pos pos))
            {
                Log.Debug("找不到位置");
                return;
            }
            PlayEffect(pos.aimPos, buff, buffHandlerVar).Coroutine();
        }
#endif
    }
#if !SERVER
    public async ETVoid PlayEffect_LockToTarget(Unit target,Buff_PlayEffect buff, BuffHandlerVar buffHandlerVar)
    {
        try
        {
            Log.Debug("播放特效");
            UnityEngine.GameObject go = null;
            go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.effectObjId);//先找到缓存的特效物体

            BuffHandlerVar.cacheDatas_object[(buffHandlerVar.source.Id, buff.buffSignal)] = go;

            go.SetActive(false);

            //在目标位置处播放,并跟随
            go.transform.position = target.Position + buff.posOffset.ToV3();
            go.transform.parent = target.GameObject.transform;

            go.SetActive(true);
            if (buff.canBeInterrupted)
                buffHandlerVar.cancelToken.Register(() =>
                {
                    go.transform.parent = null;
                    Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.effectObjId, go);
                });
            if (buff.duration > 0)
            {
                if (buff.canBeInterrupted)
                    await TimerComponent.Instance.WaitAsync((long)(buff.duration*1000), buffHandlerVar.cancelToken);
                else
                    await TimerComponent.Instance.WaitAsync(buff.duration);
                go.transform.parent = null;
                Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.effectObjId, go);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }

    public async ETVoid PlayEffect(Vector3 target, Buff_PlayEffect buff, BuffHandlerVar buffHandlerVar)
    {
        Log.Debug("播放特效");
        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.effectObjId);//先找到缓存的特效物体

        BuffHandlerVar.cacheDatas_object[(buffHandlerVar.source.Id, buff.buffSignal)] = go;

        go.SetActive(false);

        //在目标位置处播放,并跟随
        go.transform.position = target + buff.posOffset.ToV3();

        go.SetActive(true);
        if (buff.canBeInterrupted)
            buffHandlerVar.cancelToken.Register(() =>
            {
                go.transform.parent = null;
                Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.effectObjId, go);
            });
        if (buff.duration > 0)
        {
            if (buff.canBeInterrupted)
                await TimerComponent.Instance.WaitAsync((long)(buff.duration * 1000), buffHandlerVar.cancelToken);
            else
                await TimerComponent.Instance.WaitAsync(buff.duration);
        }
        go.transform.parent = null;
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.effectObjId, go);
    }
#endif

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
            var go = BuffHandlerVar.cacheDatas_object[(buffHandlerVar.source.Id, buff.buffSignal)] as UnityEngine.GameObject;
            if (go != null)
            {
                go.transform.parent = null;
                Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.effectObjId, go);
            }
        }
#endif
    }
}



