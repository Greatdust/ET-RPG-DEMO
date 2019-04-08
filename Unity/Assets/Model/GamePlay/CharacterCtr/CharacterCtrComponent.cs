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

        public AnimatorComponent animatorComponent;

        public CameraComponent cameraComponent;

        private Transform transform;

        public bool isInPlayerCtr;

        public float sendMsgInterval = 0.2f;
        public float timing;

        private CommandInput_Move input_Move;

        private NumericComponent numericComponent;

        private float moveSpeed = 0;

        public void Awake()
        {
            transform = GetParent<Unit>().GameObject.transform;
            animatorComponent = GetParent<Unit>().GetComponent<AnimatorComponent>();
            timing = sendMsgInterval;
            input_Move = new CommandInput_Move();
            numericComponent = GetParent<Unit>().GetComponent<NumericComponent>();
            bCharacterController = GetParent<Unit>().GameObject.GetComponent<BCharacterController>();
        }

        public void MoveTo(Vector3 aim)
        {
            if (Vector3.Distance(transform.position, aim) < 0.05f)
            {
                bCharacterController.SetPosition(aim);
                return;
            }

            moveTarget = aim;
            Vector3 distance = moveTarget - transform.position;
            moveDir = distance.normalized;
            //transform.forward = moveDir;
            canMove = true;
        }


        public void FixedUpdate()
        {
            Vector3 motion = Vector3.zero;
            if (isInPlayerCtr)
            {
                float moveLeft = Input.GetAxisRaw("Horizontal");
                float moveForward = Input.GetAxisRaw("Vertical");

                if (moveLeft != 0 || moveForward != 0)
                {

                    Vector3 targetDirection = new Vector3(moveLeft, 0, moveForward);
                    float y = Camera.main.transform.rotation.eulerAngles.y;
                    targetDirection = (Quaternion.Euler(0, y, 0) * targetDirection).normalized;
                    input_Move.moveDir = targetDirection; // 暂时不发送Y轴可能产生的移动信息
                    GetParent<Unit>().GetComponent<CommandComponent>().CollectCommandInput(input_Move);
                    //transform.forward = moveDir;

                    //characterController.Move(moveDir * GetParent<Unit>().GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed) * Time.deltaTime);
                }

            }

            if (!canMove)
            {
                bCharacterController.Move(Vector3.zero);
                animatorComponent.SetFloatValue("MoveSpeed", 0);
                return;
            }
            else
            {
                animatorComponent.SetFloatValue("MoveSpeed", 1);
            }
            Quaternion quaDir = Quaternion.LookRotation(moveDir, Vector3.up);
            bCharacterController.ChangeRotation(Quaternion.Slerp(transform.rotation, quaDir, Time.deltaTime * 5));
            if (Vector3.Distance(transform.position, moveTarget) < 0.05f || Vector3.Dot(moveDir, moveTarget - transform.position) < 0)
            {
                canMove = false;
                bCharacterController.SetPosition(moveTarget);
                return;
            };
            bCharacterController.Move(moveDir * Time.fixedDeltaTime * numericComponent.GetAsFloat(NumericType.Speed));


        }

        

    }
}
