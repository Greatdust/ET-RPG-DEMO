using BulletUnity;
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
    public class CharacterCtrComponentAwakeSystem : AwakeSystem<CharacterCtrComponent>
    {
        public override void Awake(CharacterCtrComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class CharacterCtrComponentUpdateSystem : FixedUpdateSystem<CharacterCtrComponent>
    {
        public override void FixedUpdate(CharacterCtrComponent self)
        {
            self.FixedUpdate();
        }
    }


    public class CharacterCtrComponent : Component
    {
        private bool canMove;
        public Vector3 moveTarget;
        public Vector3 moveDir;

        public BCharacterController bCharacterController;



        private Unit transform;

        public bool isInPlayerCtr;

        public float sendMsgInterval = 0.2f;
        public float timing;

        private CommandInput_Move input_Move;

        private NumericComponent numericComponent;

        private float moveSpeed = 0;

        public void Awake()
        {
            transform = GetParent<Unit>();
            timing = sendMsgInterval;
            input_Move = new CommandInput_Move();
            numericComponent = GetParent<Unit>().GetComponent<NumericComponent>();
            bCharacterController = GetParent<Unit>().GetComponent<BCharacterController>();
        }

        public void MoveTo(Vector3 aim)
        {
            if (Vector3.Distance(transform.Position, aim) < 0.05f)
            {
                bCharacterController.SetPosition(aim);
                return;
            }

            moveTarget = aim;
            Vector3 distance = moveTarget - transform.Position;
            //Vector3 distance = new Vector3(moveTarget.x - transform.Position.x, 0, moveTarget.z - transform.Position.z);
            moveDir = distance.normalized;
            //transform.forward = moveDir;
            canMove = true;
        }


        public void FixedUpdate()
        {
            Vector3 motion = Vector3.zero;

            if (!canMove)
            {
                bCharacterController.Move(Vector3.zero);
                return;
            }

            //Quaternion quaDir = Quaternion.LookRotation(moveDir, Vector3.up);
            //bCharacterController.ChangeRotation(Quaternion.Slerp(transform.rotation, quaDir, Time.deltaTime * 5));
            if (Vector3.Distance(transform.Position, moveTarget) < 0.05f || Vector3.Dot(moveDir, moveTarget - transform.Position) < 0)
            {
                canMove = false;
                bCharacterController.SetPosition(moveTarget);
                return;
            };
            bCharacterController.Move(moveDir * EventSystem.FixedUpdateTime * numericComponent.GetAsFloat(NumericType.Speed));


        }



    }
}
