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

    public class CharacterMoveComponent : Component
    {
        public enum MoveType
        {
            Move,
            PushedBack, //被击退
            Launched, // 被击飞
            Floated, //漂浮
            Sunk, //沉没
            Attract, // 吸引
        }

        public MoveType moveType;
        public Unit unit;
        public AnimatorComponent animatorComponent;

        public Vector3 moveTarget;
        public Vector3 startPosition;
        public float moveSpeed;
        public Vector3 moveDir;
        public Quaternion aimRotation;
        public long startTime;
        public long needTime;
        public long endTime;

        public ETTaskCompletionSource moveTcs;
        public ETCancellationTokenSource cancellationTokenSource;

        private float baseMoveSpeed = 5;// 这个应该从配置表里读

        public void Awake()
        {
            unit = GetParent<Unit>();
            animatorComponent = unit.GetComponent<AnimatorComponent>();
        }

        public async ETVoid MoveAsync( List<Vector3> path)
        {
            if (path.Count == 0)
            {
                return;
            }
            if (moveTcs != null)
            {
                moveTcs = null;
            }
            float speed = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.MoveSpeed);

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

        public ETTask MoveTo(Vector3 target, float speed)
        {
            if (Vector3.Distance(target, GetParent<Unit>().Position) < 0.05f) return ETTask.CompletedTask;
            moveTarget = target;
            float distance = Vector3.Distance(unit.Position, moveTarget);
            moveType = MoveType.Move;
            moveSpeed = speed;
            moveTcs = new ETTaskCompletionSource();
            startPosition = unit.Position;
            moveDir = (moveTarget - unit.Position).normalized;
            aimRotation = Quaternion.LookRotation(moveDir, Vector3.up);
            startTime = TimeHelper.Now();
            float time = distance / speed;
            needTime = (long)(time * 1000);
            endTime = startTime + needTime;

               
            return moveTcs.Task;

        }

        public ETTask PushBackedTo(Vector3 target, float speed)
        {
            moveTarget = target;
            float distance = Vector3.Distance(unit.Position, moveTarget);
            moveType = MoveType.PushedBack;
            moveSpeed = speed;
            moveTcs = new ETTaskCompletionSource();
            startPosition = unit.Position;
            moveDir = (moveTarget - unit.Position).normalized;
            startTime = TimeHelper.Now();
            float time = distance / speed;
            needTime = (long)(time * 1000);
            endTime = startTime + needTime;
            GetParent<Unit>().OnCollisionEnterHandler += CharacterMoveComponent_OnCollisionEnterHandler;
            animatorComponent.SetAnimatorSpeed(1);
            animatorComponent.SetTrigger(CharacterAnim.Hit);
            return moveTcs.Task;
        }

        private void CharacterMoveComponent_OnCollisionEnterHandler(Unit obj,Vector3 pos)
        {
            //撞到东西了就要停下来
            OnMoveEnd();

        }

        public void FixedUpdate()
        {
            if (moveTcs == null) return;

            var property_CharacterState = GetParent<Unit>().GetComponent<CharacterStateComponent>();
            if (property_CharacterState.Get(SpecialStateType.CantDoAction))
            {
                OnMoveEnd();
                return;
            }

            long timeNow = TimeHelper.Now();
            if (timeNow >= endTime || Vector3.Distance(unit.Position, this.moveTarget) < 0.01f)
            {
                OnMoveEnd();
                return;
            }
            if (moveType == MoveType.Move)
            {
                float pitch = moveSpeed / baseMoveSpeed;
                animatorComponent.SetAnimatorSpeed(pitch);
                animatorComponent.SetBoolValue(CharacterAnim.Run, true);
                GetParent<Unit>().GetComponent<AudioComponent>().PlayMoveSound(pitch);
                unit.Rotation = Quaternion.Slerp(unit.Rotation, aimRotation, Time.deltaTime * 15);
            }

            float amount = (timeNow - this.startTime) * 1f / needTime;
            unit.Position = Vector3.Lerp(this.startPosition, this.moveTarget, amount);
        }




        public void OnMoveEnd()
        {
            switch (moveType)
            {
                case MoveType.Move:
                    animatorComponent.SetBoolValue(CharacterAnim.Run, false);
                    GetParent<Unit>().GetComponent<AudioComponent>().PauseMoveSound();
                    animatorComponent.SetAnimatorSpeed(1);
                    break;
                case MoveType.PushedBack:
                    GetParent<Unit>().OnCollisionEnterHandler -= CharacterMoveComponent_OnCollisionEnterHandler;
                    break;
                case MoveType.Launched:
                    break;
                case MoveType.Floated:
                    break;
                case MoveType.Sunk:
                    break;
            }
            var t = moveTcs;
            moveTcs = null;
            t?.SetResult();
        }
    }
}
