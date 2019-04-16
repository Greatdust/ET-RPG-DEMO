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
#if !SERVER
        Buff_HitEffect buff = (Buff_HitEffect)buffHandlerVar.data;
        if (buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targets))
        {

            foreach (var v in targets.targets)
            {
                //未造成伤害就不给予效果
                if (buffHandlerVar.GetBufferValue(out BufferValue_AttackSuccess attackSuccess))
                {
                    if (!attackSuccess.successDic[v.Id]) continue;
                }
                PlayEffect(buff, v.Position).Coroutine();
            }
            return;
        }


        if (!buffHandlerVar.GetBufferValue(out BufferValue_Pos value_Pos))
        {
            return;
        }
        PlayEffect(buff, value_Pos.aimPos).Coroutine();

#endif
    }
#if !SERVER
    async ETVoid PlayEffect(Buff_HitEffect buff, Vector3 pos)
    {
        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.hitObjId);//先找到缓存的特效物体
        var effectGo = go;
        go.SetActive(false);

        //在目标位置处播放
        go.transform.position = pos + buff.posOffset.ToV3();


        go.SetActive(true);
        await TimerComponent.Instance.WaitAsync((long)(buff.duration * 1000));
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.hitObjId, effectGo);
    }
#endif
}



