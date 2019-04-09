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


    public class CharacterMoveComponent : Component
    {
        public Unit unit;

        public Vector3 moveTarget;
        public Vector3 startPosition;
        public Vector3 moveDir;
        public Quaternion aimRotation;
        public long startTime;
        public long needTime;
        public long endTime;

        public ETTaskCompletionSource moveTcs;
        public ETCancellationTokenSource cancellationTokenSource;


        public void Awake()
        {
            unit = GetParent<Unit>();

        }

        public async ETTask MoveAsync(List<Vector3> path)
        {

            if (path.Count == 0)
            {
                return;
            }
            if (moveTcs != null) moveTcs = null;

            float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed);

            if (Vector3.Distance(unit.Position, path[0]) > 1)
            {
                //直接拉扯
                unit.Position = path[0];
            }

            // 第一个点是unit的当前位置，所以不用发送
            for (int i = 1; i < path.Count; ++i)
            {
                Vector3 v3 = path[i];
                await MoveTo(v3, speed);
            }

        }

        ETTask MoveTo(Vector3 target, float speed)
        {
            if (Vector3.Distance(target, moveTarget) < 0.01f) return ETTask.CompletedTask;
            moveTarget = target;
            float distance = Vector3.Distance(unit.Position, moveTarget);
            if (distance < 0.01f) return ETTask.CompletedTask;
            moveTcs = new ETTaskCompletionSource();
            startPosition = unit.Position;
            moveDir = (unit.Position - moveTarget).normalized;
            aimRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            startTime = TimeHelper.Now();
            float time = distance / speed;
            needTime = (long)(time * 1000);
            endTime = startTime + needTime;

            return moveTcs.Task;

        }

        public void FixedUpdate()
        {
            if (moveTcs == null) return;

            long timeNow = TimeHelper.Now();
            if (timeNow >= endTime)
            {

                unit.Position = moveTarget;
                var t = moveTcs;
                moveTcs = null;
                t.SetResult();
                return;
            }

            unit.Rotation = Quaternion.Slerp(unit.Rotation, aimRotation, EventSystem.FixedUpdateTime);

            float amount = (timeNow - this.startTime) * 1f / needTime;
            unit.Position = Vector3.Lerp(this.startPosition, this.moveTarget, amount);
        }
    }
}
