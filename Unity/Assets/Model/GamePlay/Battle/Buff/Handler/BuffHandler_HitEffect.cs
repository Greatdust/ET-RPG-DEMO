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
    public async void ActionHandle(IBuffData data, Unit source, Unit target)
    {

        Buff_HitEffect buff_HitEffect = data as Buff_HitEffect;
        //特效
        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff_HitEffect.hitObjId);//先找到缓存的特效物体
        var effectGo = go;
        go.SetActive(false);

        //在目标位置处播放
        go.transform.position = target.GameObject.transform.position;


        go.SetActive(true);
        await TimerComponent.Instance.WaitAsync((long)(buff_HitEffect.duration * 1000));
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff_HitEffect.hitObjId, effectGo);
    }

    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            ActionHandle(data, source, target);
        }
    }
}



