using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PF;

[BuffType(BuffIdType.PlayEffect)]
public class BuffHandler_PlayEffect : BaseBuffHandler,IBuffActionWithGetInputHandler,IBuffRemoveHanlder
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        Buff_PlayEffect buff = (Buff_PlayEffect)buffHandlerVar.data;
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }
        foreach (var v in targetUnits.targets)
        {


            UnityEngine.GameObject go = null;
            go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.effectObjId);//先找到缓存的特效物体

            BuffHandlerVar.cacheDatas_object[(buffHandlerVar.source.Id, buff.buffSignal)] = go;

            go.SetActive(false);

            //在目标位置处播放,并跟随
            go.transform.position = v.Position + buff.posOffset;
            go.transform.parent = v.GameObject.transform;

            go.SetActive(true);
        }
#endif
    }

    public void Remove(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        Buff_PlayEffect buff = (Buff_PlayEffect)buffHandlerVar.data;
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }
        foreach (var v in targetUnits.targets)
        {
            var go = BuffHandlerVar.cacheDatas_object[(buffHandlerVar.source.Id, buff.buffSignal)] as UnityEngine.GameObject;
            go.transform.parent = null;
            Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.effectObjId, go);
        }
#endif
    }
}



