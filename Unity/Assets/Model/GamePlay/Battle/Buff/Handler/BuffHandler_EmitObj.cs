using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using PF;
using UnityEngine;
using Box2DSharp.Collision.Shapes;

[BuffType(BuffIdType.EmitObj)]
public class BuffHandler_EmitObj : BaseBuffHandler,IBuffActionWithGetInputHandler
{

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_EmitObj buff = buffHandlerVar.data as Buff_EmitObj;

        if (buff.lockTarget)
        {
            if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
            {
                Log.Error("没有找到锁定的目标");
                return;
            }
            //每个单位都发射一个特效
            foreach (var v in targetUnits.targets)
            {

#if !SERVER
                //特效
                UnityEngine.GameObject go = null;
                go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.emitObjId);//先找到缓存的特效物体
                var effectGo = go;
                go.SetActive(false);
                Unit unit = UnitFactory.CreateEmitObj(go, buff.layer, buff.layerMask, new CircleShape() { Radius = 0.5f });

#else

#endif
                unit.Position = unit.Position + buff.startPosOffset;
                CollisionEvent_LockTarget(buffHandlerVar, unit, v, buff.emitSpeed).Coroutine();
            }
        }
        else
        {
            if (!buffHandlerVar.GetBufferValue(out BufferValue_Dir dir))
            {
                Log.Error("发射飞行道具没有传入方向!");
                return;
            }
#if !SERVER
            UnityEngine.GameObject go = null;
            go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.emitObjId);//先找到缓存的特效物体
            var effectGo = go;
            go.SetActive(false);
            Unit unit = UnitFactory.CreateEmitObj(go, buff.layer, buff.layerMask, new CircleShape() { Radius = 0.5f });

#else

#endif

            unit.Position = unit.Position + buff.startPosOffset;
            Quaternion quaternion = Quaternion.LookRotation(dir.dir, Vector3.up);
            unit.Rotation = quaternion;
            Vector3 targetPos = dir.dir * buff.emitSpeed * buff.duration + unit.Position;
            CollisionEvent(buffHandlerVar, unit, targetPos, buff.emitSpeed).Coroutine();
        }

    }

    async ETVoid CollisionEvent_LockTarget(BuffHandlerVar buffHandlerVar, Unit emitObj, Unit target, float speed)
    {
        Buff_EmitObj buff = buffHandlerVar.data as Buff_EmitObj;

        var result = await emitObj.GetComponent<EmitObjMoveComponent>().MoveTo(target, speed);
        BuffHandlerVar newVar = buffHandlerVar;

        newVar.bufferValues = new Dictionary<Type, IBufferValue>();
        //万一提前被其他人挡了
        newVar.bufferValues[typeof(BufferValue_TargetUnits)] = new BufferValue_TargetUnits() { targets = new Unit[] { result.Item1 } };

        var go = emitObj.GameObject;
        emitObj.RemoveGameObject();
        emitObj.Dispose();
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.emitObjId, go);

        SkillHelper.collisionActions[buff.pipelineSignal](newVar);


    }

    async ETVoid CollisionEvent(BuffHandlerVar buffHandlerVar, Unit emitObj, Vector3 targetPos, float speed)
    {
        Buff_EmitObj buff = buffHandlerVar.data as Buff_EmitObj;

        var result = await emitObj.GetComponent<EmitObjMoveComponent>().MoveTo(targetPos, speed);
        BuffHandlerVar newVar = buffHandlerVar;

        newVar.bufferValues = new Dictionary<Type, IBufferValue>();
        //万一提前被其他人挡了
        if (result.Item1 != null)
            newVar.bufferValues[typeof(BufferValue_TargetUnits)] = new BufferValue_TargetUnits() { targets = new Unit[] { result.Item1 } };
        newVar.bufferValues[typeof(BufferValue_Pos)] = new BufferValue_Pos() { aimPos = result.Item2 };


        var go = emitObj.GameObject;
        emitObj.RemoveGameObject();
        emitObj.Dispose();
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.emitObjId, go);


        SkillHelper.collisionActions[buff.pipelineSignal](newVar);
    }

}



