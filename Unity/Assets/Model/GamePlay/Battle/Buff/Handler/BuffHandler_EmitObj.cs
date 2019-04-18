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
        UnitData emitObjData = new UnitData();
        if (!buff.FindFriend)
            emitObjData.groupIndex = buffHandlerVar.source.UnitData.groupIndex;
        else
        {
            switch (buffHandlerVar.source.UnitData.groupIndex)
            {
                case GroupIndex.Default:
                    emitObjData.groupIndex = GroupIndex.Default;
                    break;
                case GroupIndex.Player:
                    emitObjData.groupIndex = GroupIndex.Monster;
                    break;
                case GroupIndex.Monster:
                    emitObjData.groupIndex = GroupIndex.Player;
                    break;
            }
        }
        emitObjData.unitLayer = buff.layer;
        emitObjData.layerMask = buff.layerMask;
        emitObjData.unitTag = UnitTag.Default;
        if (buff.lockTarget)
        {
            if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
            {
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

                Unit unit = UnitFactory.CreateEmitObj(go, emitObjData);
                go.SetActive(true);
#else

                Unit unit = ETHotfix.UnitFactory.CreateEmitObj(emitObjData,buff.emitObjId);
#endif
                Vector3 dir = (v.Position - buffHandlerVar.source.Position).normalized;
                Vector3 startPosOffset = buff.startPosOffset.ToV3();

                unit.Position = buffHandlerVar.source.Position + new Vector3(dir.x * startPosOffset.x, startPosOffset.y, dir.z * startPosOffset.z);
                Quaternion quaternion = Quaternion.LookRotation(dir, Vector3.up);
                unit.Rotation = quaternion;
                CollisionEvent_LockTarget(buffHandlerVar, unit, v, buff.emitSpeed).Coroutine();
            }
        }
        else
        {
            if (!buffHandlerVar.GetBufferValue(out BufferValue_Dir buffer_dir))
            {
                return;
            }
#if !SERVER
            UnityEngine.GameObject go = null;
            go = Game.Scene.GetComponent<EffectCacheComponent>().Get(buff.emitObjId);//先找到缓存的特效物体
            var effectGo = go;
            Unit unit = UnitFactory.CreateEmitObj(go, emitObjData);
            go.SetActive(true);
#else
            Unit unit = ETHotfix.UnitFactory.CreateEmitObj(emitObjData, buff.emitObjId);
#endif
            Vector3 startPosOffset = buff.startPosOffset.ToV3();
            Vector3 dir = buffer_dir.dir.normalized;

            unit.Position = buffHandlerVar.source.Position + new Vector3(dir.x * startPosOffset.x, startPosOffset.y, dir.z * startPosOffset.z);
            //Log.Debug(string.Format("{0}使用者位置 方向{1} 初始位置偏移量{2},计算出的最终位置{3}", buffHandlerVar.source.Position, dir, startPosOffset, unit.Position));
            //Log.Debug(string.Format("飞行物体的高度{0}", unit.Position.y));


            Quaternion quaternion = Quaternion.LookRotation(buffer_dir.dir, Vector3.up);
            unit.Rotation = quaternion;
            buffHandlerVar.source.Rotation = quaternion;
            Vector3 targetPos = dir * buff.emitSpeed * buff.duration + unit.Position;
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
        newVar.bufferValues[typeof(BufferValue_TargetUnits)] = new BufferValue_TargetUnits() { targets = new Unit[] { result.Item1 } }; // 产出目标单位
        newVar.bufferValues[typeof(BufferValue_Pos)] = new BufferValue_Pos() { aimPos = result.Item2 }; // 产出碰撞位置
        Vector3 dir = (result.Item2 - emitObj.Position).normalized;
        newVar.bufferValues[typeof(BufferValue_Dir)] = new BufferValue_Dir() { dir = new Vector3(dir.x,0, dir.z) }; // 产出方向,为了实现击退等效果
#if !SERVER
        var go = emitObj.GameObject;
        emitObj.RemoveGameObject();
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.emitObjId, go);
#endif
        emitObj.Dispose();
        SkillHelper.collisionActions[(buffHandlerVar.source,buff.pipelineSignal)](newVar);


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
        Vector3 dir = (result.Item2 - emitObj.Position).normalized;
        newVar.bufferValues[typeof(BufferValue_Dir)] = new BufferValue_Dir() { dir = new Vector3(dir.x,0,dir.z) };
#if !SERVER
        var go = emitObj.GameObject;
        emitObj.RemoveGameObject();
        Game.Scene.GetComponent<EffectCacheComponent>().Recycle(buff.emitObjId, go);
#endif
        emitObj.Dispose();

        SkillHelper.collisionActions[(buffHandlerVar.source, buff.pipelineSignal)](newVar);
    }

}



