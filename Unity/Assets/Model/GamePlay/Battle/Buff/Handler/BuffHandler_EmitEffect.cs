using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PF;
using UnityEngine;

[BuffType(BuffIdType.EmitEffectInSkill)]
public class BuffHandler_EmitEffect : BaseBuffHandler, IBuffActionWithCollision,IBuffActionWithGetInputHandler
{

    public async void ActionHandle(Buff_EmitEffect buff_emitEffect, Unit source, Unit target,float playSpeed, Action<long> collisionEvent)
    {
        //特效
        UnityEngine.GameObject go = null;
        go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff_emitEffect.emitObjId);//先找到缓存的特效物体
        var effectGo = go;
        go.SetActive(false);
        //特效携带的碰撞事件的添加

        if (collisionEvent != null)
        {
            CollsionHelper collsionHelper = go.GetComponentInChildren<CollsionHelper>(true);
            collsionHelper.onCollisionEnter.Add(collisionEvent);
        }
        //计算特效的播放


        if (buff_emitEffect.lockTarget)
        {
            //锁定目标,一般是发射期以一个单位作为目标


            if (!buff_emitEffect.reverseDir && buff_emitEffect.emitSpeed == 0)
            {
                //锁定目标+ 正方向 + 飞行速度=0,等于直接在目标处播放特效
                go.transform.localPosition = target.GameObject.transform.position + (UnityEngine.Vector3)buff_emitEffect.emitStartPos;
                if (target == source)
                    go.transform.forward = source.GameObject.transform.forward;
                else
                go.transform.forward = source.GameObject.transform.position - target.GameObject.transform.position;
            }
            else 
            {
                //锁定目标+ 反方向,等于
                go.transform.position = target.GameObject.transform.position + (UnityEngine.Vector3)buff_emitEffect.emitStartPos;
                if (target == source)
                    go.transform.forward = source.GameObject.transform.forward;
                else
                    go.transform.forward = source.GameObject.transform.position - target.GameObject.transform.position;
            }
        }

        go.SetActive(true);
        if (buff_emitEffect.emitSpeed != 0)
        {
            Vector3 targetPostion = Vector3.zero;
            if (buff_emitEffect.lockTarget)
            {
                if (!buff_emitEffect.reverseDir)
                    targetPostion = target.GameObject.transform.position;
                else
                    targetPostion = source.GameObject.transform.position;
            }
            else
            {
                if (buff_emitEffect.emitDir == Vector3.zero)
                {
                    targetPostion = go.transform.position + go.transform.forward * buff_emitEffect.emitSpeed * buff_emitEffect.duration;
                }
                else
                {
           
                    targetPostion = go.transform.position + (go.transform.forward +
                        go.transform.forward * buff_emitEffect.emitDir.x +
                        go.transform.right * buff_emitEffect.emitDir.y +
                        go.transform.up * buff_emitEffect.emitDir.z) * buff_emitEffect.emitSpeed * buff_emitEffect.duration;
                    go.transform.forward = buff_emitEffect.emitDir;
                }
            }
            go.transform.LeanMove(targetPostion, buff_emitEffect.duration / playSpeed);
        }
        UnityEngine.ParticleSystem.MainModule mainModule = effectGo.GetComponentInChildren<UnityEngine.ParticleSystem>().main;
        mainModule.simulationSpeed = playSpeed;//这里实际上是内部调用了C++端,所以改变结构体里的值直接影响结果
        await TimerComponent.Instance.WaitAsync(buff_emitEffect.duration / playSpeed);
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff_emitEffect.emitObjId, effectGo);
    }

    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues, Action<long> action)
    {
        Buff_EmitEffect buff_EmitEffect = data as Buff_EmitEffect;
        foreach (var v in baseBuffReturnedValues)
        {
            BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = v as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            ActionHandle(buff_EmitEffect, source, target,buffReturnedValue_TargetUnit.Value.playSpeedScale, action);
        }

    }

    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        ActionHandle(data, source, baseBuffReturnedValues, null);
    }
}



