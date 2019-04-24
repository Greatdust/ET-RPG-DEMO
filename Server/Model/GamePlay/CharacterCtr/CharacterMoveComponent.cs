using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{

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

        }

      
    }
}
