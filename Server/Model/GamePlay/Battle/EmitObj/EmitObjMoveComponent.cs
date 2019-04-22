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
    }

}
