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
    public class CharacterCtrComponentUpdateSystem : UpdateSystem<CharacterCtrComponent>
    {
        public override void Update(CharacterCtrComponent self)
        {
            self.Update();
        }
    }


    public class CharacterCtrComponent : Component
    {
        public CharacterController characterController;//一定要和角色根节点绑在一次

        private bool canMove;
        public float moveSpeed =5;
        public Vector3 moveTarget;
        public Vector3 moveDir;

        public float verticalSpeed = -5;

        public AnimatorComponent animatorComponent;

        public CameraComponent cameraComponent;

        private Transform transform;

        public readonly Unit_Move unit_MoveMsg = new Unit_Move();

        public bool isInPlayerCtr;

        public float sendMsgInterval = 0.2f;
        public float timing;


        public void Awake()
        {
            characterController = GetParent<Unit>().GameObject.GetComponentInChildren<CharacterController>();
            transform = GetParent<Unit>().GameObject.transform;
            animatorComponent = GetParent<Unit>().GetComponent<AnimatorComponent>();
            timing = sendMsgInterval;
        }

        public void MoveTo(Vector3 aim)
        {
            if (isInPlayerCtr) return;
            moveTarget = aim;
            moveDir = (moveTarget - transform.position).normalized;
            transform.forward = moveDir;
            canMove = true;
        }


        public void Update()
        {
            if (!characterController.isGrounded)
            {
                Vector3 motion = transform.up * verticalSpeed * Time.deltaTime;
                characterController.Move(motion);
            }


            if (isInPlayerCtr)
            {
                float moveLeft = Input.GetAxisRaw("Horizontal");
                float moveForward = Input.GetAxisRaw("Vertical");

                if (moveLeft != 0 || moveForward != 0)
                {

                    animatorComponent.SetFloatValue("MoveSpeed", 1f);

                    Vector3 targetDirection = new Vector3(moveLeft, 0, moveForward);
                    float y = Camera.main.transform.rotation.eulerAngles.y;
                    targetDirection = Quaternion.Euler(0, y, 0) * targetDirection;


                    Vector3 motion = targetDirection * moveSpeed * Time.deltaTime;

                    transform.forward = motion;
                    characterController.Move(motion);
                }
                else
                {
                    animatorComponent.SetFloatValue("MoveSpeed", 0f);
                }
                timing += Time.deltaTime;
                if (timing >= sendMsgInterval)
                {
                    timing = 0;
                    unit_MoveMsg.MoveX = transform.position.x;
                    unit_MoveMsg.MoveY = transform.position.y;
                    unit_MoveMsg.MoveZ = transform.position.z;

                    unit_MoveMsg.EulerX = transform.forward.x;
                    unit_MoveMsg.EulerY = transform.forward.y;
                    unit_MoveMsg.EulerZ = transform.forward.z;

                    SessionComponent.Instance.Session.Send(unit_MoveMsg);
                }

         

            }
            else
            {
                if (!canMove)
                {
                    return;
                }
                if (Vector3.Distance(transform.position, moveTarget) < 0.1f || Vector3.Dot(moveDir,moveTarget - transform.position)<0)
                {
                    canMove = false;
                    transform.position = moveTarget;
                    animatorComponent.SetFloatValue("MoveSpeed", 0f);
                    return;
                }
                animatorComponent.SetFloatValue("MoveSpeed", 1f);
                Vector3 motion = moveDir * moveSpeed * Time.deltaTime;
                characterController.Move(motion);
            }


        }

    }
}
