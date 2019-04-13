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

        BufferValue_Pos bufferValue_Pos = (BufferValue_Pos)buffHandlerVar.bufferValues[typeof(BufferValue_Pos)];

        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.effectObjId);//先找到缓存的特效物体

        BuffHandlerVar.cacheDatas_object[(buffHandlerVar.source.Id, buff.buffSignal)] = go;

        go.SetActive(false);

        //在目标位置处播放
        go.transform.position = bufferValue_Pos.aimPos + buff.posOffset;

        go.SetActive(true);
#endif
    }

    public void Remove(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        Buff_PlayEffect buff = (Buff_PlayEffect)buffHandlerVar.data;

        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.effectObjId, BuffHandlerVar.cacheDatas_object[(buffHandlerVar.source.Id, buff.buffSignal)] as UnityEngine.GameObject);
#endif
    }
}



