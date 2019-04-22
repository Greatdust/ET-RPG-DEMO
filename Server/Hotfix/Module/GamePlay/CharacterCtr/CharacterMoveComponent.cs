using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static ETModel.CharacterMoveComponent;


    [ObjectSystem]
    public class CharacterMoveComponentAwakeSystem : AwakeSystem<CharacterMoveComponent>
    {
        public override void Awake(CharacterMoveComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class CharacterMoveComponentUpdateSystem : FixedUpdateSystem<CharacterMoveComponent>
    {
        public override void FixedUpdate(CharacterMoveComponent self)
        {
            self.FixedUpdate();
        }
    }

    [Event(EventIdType.CancelPreAction)]
    public class CancelMoveEvent : AEvent<Unit>
    {
        public override void Run(Unit unit)
        {
            var CharacterMoveComponent = unit.GetComponent<CharacterMoveComponent>();
            if (CharacterMoveComponent.moveTcs != null)
            {
                CharacterMoveComponent.moveTcs = null;
                CharacterMoveComponent.OnMoveEnd();
            }
        }
    }

public static class CharacterMoveComponentSystem
{

    public static async ETVoid MoveAsync(this CharacterMoveComponent self, List<Vector3> path)
    {
        if (path.Count == 0)
        {
            return;
        }

        if (self.moveTcs != null)
        {
            self.moveTcs = null;
        }
        float speed = self.unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.MoveSpeed);

        if (Vector3.Distance(self.unit.Position, path[0]) > 1)
        {
            //直接拉扯
            self.unit.Position = path[0];
        }


        // 第一个点是unit的当前位置，所以不用发送
        for (int i = 1; i < path.Count; ++i)
        {
            Vector3 v3 = path[i];
            await self.MoveTo(v3, speed);
        }

    }

    public static ETTask MoveTo(this CharacterMoveComponent self, Vector3 target, float speed)
    {
        self.moveTarget = target;
        float distance = Vector3.Distance(self.unit.Position, self.moveTarget);

        if (distance < 0.02f) return ETTask.CompletedTask;

        self.moveType = MoveType.Move;
        self.moveSpeed = speed;
        self.moveTcs = new ETTaskCompletionSource();
        self.startPosition = self.unit.Position;
        self.moveDir = (self.moveTarget - self.unit.Position).normalized;
        self.aimRotation = Quaternion.LookRotation(self.moveDir, Vector3.up);
        self.startTime = TimeHelper.Now();
        float time = distance / speed;
        self.needTime = (long)(time * 1000);
        self.endTime = self.startTime + self.needTime;

        return self.moveTcs.Task;

    }

    public static ETTask PushBackedTo(this CharacterMoveComponent self, Vector3 target, float speed)
    {
        self.moveTarget = target;


        self.RaycastPushBackPos(ref self.moveTarget);

        float distance = Vector3.Distance(self.unit.Position, self.moveTarget);
        self.moveType = MoveType.PushedBack;
        self.moveSpeed = speed;

        self.startPosition = self.unit.Position;
        self.moveDir = (self.moveTarget - self.unit.Position).normalized;

        self.moveTcs = new ETTaskCompletionSource();
        self.startTime = TimeHelper.Now();
        float time = distance / speed;
        self.needTime = (long)(time * 1000);
        self.endTime = self.startTime + self.needTime;
        M2C_Pushback m2C = new M2C_Pushback();
        m2C.Frame = Game.Scene.GetComponent<UnitStateMgrComponent>().currFrame;
        m2C.Id = self.unit.Id;
        m2C.MoveTarget = self.moveTarget.ToV3Info();
        m2C.Time = time;
        ETHotfix.MessageHelper.Broadcast(m2C);
        Log.Debug("击退,击退用时" + time);
        return self.moveTcs.Task;
    }

    static void RaycastPushBackPos(this CharacterMoveComponent self, ref Vector3 moveTarget)
    {
        RayCastStaticObjCallback rayCast = new RayCastStaticObjCallback();
        Game.Scene.GetComponent<PhysicWorldComponent>().world.RayCast(rayCast, self.unit.Position.ToVector2(), moveTarget.ToVector2());
        if (rayCast.Hit)
        {
            var dir = moveTarget - self.unit.Position;
            moveTarget = (rayCast.Point - dir.normalized.ToVector2() * self.unit.GetComponent<P2DBodyComponent>().fixture.Shape.Radius).ToVector3(moveTarget.y);

            Log.Debug(string.Format("射线检测点+{0}  ", moveTarget));
        }
    }



    public static void FixedUpdate(this CharacterMoveComponent self)
    {
        if (self.moveTcs != null)
        {

            var property_CharacterState = self.unit.GetComponent<CharacterStateComponent>();
            if (property_CharacterState.Get(SpecialStateType.CantDoAction))
            {
                Log.Debug("角色无法行动!");
                self.OnMoveEnd();
                return;
            }

            long timeNow = TimeHelper.Now();
            if (timeNow >= self.endTime || Vector3.Distance(self.unit.Position, self.moveTarget) < 0.01f)
            {
                self.OnMoveEnd();
                return;
            }

            if (self.moveType == MoveType.Move)
            {


                self.unit.Rotation = Quaternion.Slerp(self.unit.Rotation, self.aimRotation, EventSystem.FixedUpdateTime * 15);
            }

            float amount = (timeNow - self.startTime) * 1f / self.needTime;
            self.unit.Position = Vector3.Lerp(self.startPosition, self.moveTarget, amount);
        }
    }




    public static void OnMoveEnd(this CharacterMoveComponent self)
    {
        //var Body = GetParent<Unit>().GetComponent<P2DBodyComponent>().body;
        //Body.SetLinearVelocity(System.Numerics.Vector2.Zero);
        switch (self.moveType)
        {
            case MoveType.Move:

                break;
            case MoveType.PushedBack:

                break;
            case MoveType.Launched:
                break;
            case MoveType.Floated:
                break;
            case MoveType.Sunk:
                break;
        }
        var t = self.moveTcs;
        self.moveTcs = null;
        t?.SetResult();
    }
}

