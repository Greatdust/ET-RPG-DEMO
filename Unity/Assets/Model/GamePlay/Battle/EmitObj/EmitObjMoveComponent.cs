using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class EmitObjMoveComponentAwakeSystem : AwakeSystem<EmitObjMoveComponent>
    {
        public override void Awake(EmitObjMoveComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class EmitObjMoveComponentUpdateSystem : FixedUpdateSystem<EmitObjMoveComponent>
    {
        public override void FixedUpdate(EmitObjMoveComponent self)
        {
            self.FixedUpdate();
        }
    }

    // 发射的飞行道具 的移动管理
    public class EmitObjMoveComponent : Component
    {
        public Unit follow;
        public bool isInFollow;

        public Vector3 moveTarget;
        public float moveSpeed;
        public Vector3 startPosition;
        public Vector3 moveDir;
        public Quaternion aimRotation;
        public long startTime;
        public long needTime;
        public long endTime;

        public ETTaskCompletionSource<(Unit,Vector3)> moveTCS; // 碰撞到谁了,第二个是位置

        public void Awake()
        {
   
        }

        public ETTask<(Unit, Vector3)> MoveTo(Vector3 target, float speed)
        {
            moveTarget = target;
            float distance = Vector3.Distance(GetParent<Unit>().Position, moveTarget);
            moveTCS = new ETTaskCompletionSource<(Unit, Vector3)>();
            startPosition = GetParent<Unit>().Position;
            moveDir = (moveTarget - GetParent<Unit>().Position).normalized;
            aimRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            startTime = TimeHelper.Now();
            moveSpeed = speed;
            float time = distance / speed;
            needTime = (long)(time * 1000);
            endTime = startTime + needTime;
            isInFollow = false;

            GetParent<Unit>().OnCollisionEnterHandler += OnCollisionEnterHandler;


            return moveTCS.Task;
        }
        public ETTask<(Unit, Vector3)> MoveTo(Unit target, float speed)
        {
            moveTarget = target.Position;
            this.follow = target;
            float distance = Vector3.Distance(GetParent<Unit>().Position, moveTarget);
            moveTCS = new ETTaskCompletionSource<(Unit, Vector3)>();
            startPosition = GetParent<Unit>().Position;
            moveDir = (moveTarget - GetParent<Unit>().Position).normalized;
            aimRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            startTime = TimeHelper.Now();
            moveSpeed = speed;
            float time = distance / speed;
            needTime = (long)(time * 1000);
            endTime = startTime + needTime;
            isInFollow = true;

            GetParent<Unit>().OnCollisionEnterHandler += OnCollisionEnterHandler;


            return moveTCS.Task;
        }



        private void OnCollisionEnterHandler(Unit obj,Vector3 pos)
        {
            OnEnd((obj, pos));
        }

        public void FixedUpdate()
        {
            if (moveTCS == null) return;

            long timeNow = TimeHelper.Now();
            if (!isInFollow)
            {

                if (timeNow >= endTime || Vector3.Distance(GetParent<Unit>().Position, this.moveTarget) < 0.01f)
                {

                    OnEnd((null,this.moveTarget));

                    return;
                }
            }
            else
            {
                moveTarget = follow.Position;
                float distance = Vector3.Distance(GetParent<Unit>().Position, moveTarget);
                moveDir = (moveTarget - GetParent<Unit>().Position).normalized;
                aimRotation = Quaternion.LookRotation(moveDir, Vector3.up);
                float time = distance / moveSpeed;
                needTime = (long)(time * 1000);
                endTime = startTime + needTime;
            }

            GetParent<Unit>().Rotation = Quaternion.Slerp(GetParent<Unit>().Rotation, aimRotation, Time.deltaTime * 5);

            float amount = (timeNow - this.startTime) * 1f / needTime;
            GetParent<Unit>().Position = Vector3.Lerp(this.startPosition, this.moveTarget, amount);
        }

        void OnEnd((Unit, Vector3) unit)
        {
            GetParent<Unit>().OnCollisionEnterHandler -= OnCollisionEnterHandler;
            var t = moveTCS;
            moveTCS = null;

            t.SetResult(unit);
        }
    }

}
