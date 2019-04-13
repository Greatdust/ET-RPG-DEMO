using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PF;

[BuffType(BuffIdType.HitEffect)]
public class BuffHandler_HitEffect : BaseBuffHandler,IBuffActionWithGetInputHandler
{

    public async void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_HitEffect buff = (Buff_HitEffect)buffHandlerVar.data;

        BufferValue_Pos bufferValue_Pos = (BufferValue_Pos)buffHandlerVar.bufferValues[typeof(BufferValue_Pos)];

        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.hitObjId);//先找到缓存的特效物体
        var effectGo = go;
        go.SetActive(false);

        //在目标位置处播放
        go.transform.position = bufferValue_Pos.aimPos;


        go.SetActive(true);
        await TimerComponent.Instance.WaitAsync((long)(buff.duration * 1000));
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.hitObjId, effectGo);
    }
}



