using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


[ObjectSystem]
public class EmitObjMoveComponentUpdateSystem : FixedUpdateSystem<EmitObjMoveComponent>
{
    public override void FixedUpdate(EmitObjMoveComponent self)
    {
        self.FixedUpdate();
    }
}

public static class EmitObjComponentSystem
{

    public static ETTask<(Unit, Vector3)> MoveTo(this EmitObjMoveComponent self, Vector3 target, float speed)
    {
        self.moveTarget = target;
        float distance = Vector3.Distance(self.GetParent<Unit>().Position, self.moveTarget);
        self.moveTCS = new ETTaskCompletionSource<(Unit, Vector3)>();
        self.startPosition = self.GetParent<Unit>().Position;
        self.moveDir = (self.moveTarget - self.GetParent<Unit>().Position).normalized;
        self.aimRotation = Quaternion.LookRotation(self.moveDir, Vector3.up);
        self.startTime = TimeHelper.Now();
        self.moveSpeed = speed;
        float time = distance / speed;
        self.needTime = (long)(time * 1000);
        self.endTime = self.startTime + self.needTime;
        self.isInFollow = false;

        self.GetParent<Unit>().OnCollisionEnterHandler += (obj, pos) =>
        {
            self.OnCollisionEnterHandler(obj, pos);
        };


        return self.moveTCS.Task;
    }
    public static ETTask<(Unit, Vector3)> MoveTo(this EmitObjMoveComponent self, Unit target, float speed)
    {
        self.moveTarget = target.Position;
        self.follow = target;
        float distance = Vector3.Distance(self.GetParent<Unit>().Position, self.moveTarget);
        self.moveTCS = new ETTaskCompletionSource<(Unit, Vector3)>();
        self.startPosition = self.GetParent<Unit>().Position;
        self.moveDir = (self.moveTarget - self.GetParent<Unit>().Position).normalized;
        self.aimRotation = Quaternion.LookRotation(self.moveDir, Vector3.up);
        self.startTime = TimeHelper.Now();
        self.moveSpeed = speed;
        float time = distance / speed;
        self.needTime = (long)(time * 1000);
        self.endTime = self.startTime + self.needTime;
        self.isInFollow = true;
        //联网模式,客户端的这里没用.得靠服务器发过来的消息才能确定是否和什么碰撞了
        self.GetParent<Unit>().OnCollisionEnterHandler += (obj,pos)=>
        {
            self.OnCollisionEnterHandler(obj, pos);
        };


        return self.moveTCS.Task;
    }



    private static void OnCollisionEnterHandler(this EmitObjMoveComponent self, Unit obj, Vector3 pos)
    {
        self.OnEnd((obj, pos));
    }

    public static void FixedUpdate(this EmitObjMoveComponent self)
    {
        if (self.moveTCS == null) return;

        long timeNow = TimeHelper.Now();
        if (!self.isInFollow)
        {

            if (timeNow >= self.endTime || Vector3.Distance(self.GetParent<Unit>().Position, self.moveTarget) < 0.01f)
            {

                self.OnEnd((null, self.moveTarget));

                return;
            }
        }
        else
        {
            self.moveTarget = self.follow.Position;
            float distance = Vector3.Distance(self.GetParent<Unit>().Position, self.moveTarget);
            self.moveDir = (self.moveTarget - self.GetParent<Unit>().Position).normalized;
            self.aimRotation = Quaternion.LookRotation(self.moveDir, Vector3.up);
            float time = distance / self.moveSpeed;
            self.needTime = (long)(time * 1000);
            self.endTime = self.startTime + self.needTime;
        }

        self.GetParent<Unit>().Rotation = Quaternion.Slerp(self.GetParent<Unit>().Rotation, self.aimRotation, EventSystem.FixedUpdateTime * 5);

        float amount = (timeNow - self.startTime) * 1f / self.needTime;
        self.GetParent<Unit>().Position = Vector3.Lerp(self.startPosition, self.moveTarget, amount);
    }

    public static void OnEnd(this EmitObjMoveComponent self, (Unit, Vector3) unit)
    {
        var t = self.moveTCS;
        self.moveTCS = null;

        t?.SetResult(unit);
    }
}
