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
        public Vector3 moveTarget;
        public Vector3 moveDir;

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
            characterController = GetParent<Unit>().GameObject.GetComponentInChildren<CharacterController>();
            transform = GetParent<Unit>().GameObject.transform;
            animatorComponent = GetParent<Unit>().GetComponent<AnimatorComponent>();
            timing = sendMsgInterval;
            input_Move = new CommandInput_Move();
            numericComponent = GetParent<Unit>().GetComponent<NumericComponent>();
        }

        public void MoveTo(Vector3 aim)
        {
            if (Vector3.Distance(moveTarget, aim) < 0.2f)
            {
                return;
            }
            moveTarget = aim;
            Vector3 distance = new Vector3(moveTarget.x - transform.position.x, 0, moveTarget.z - transform.position.z);
            moveDir = distance.normalized;
            Quaternion quaDir = Quaternion.LookRotation(moveDir, Vector3.up);
            //位置差距小于0.1f ,角度很接近,那就不同步了
            if (distance.magnitude < 0.1f && Mathf.Abs(Quaternion.Angle(transform.rotation, quaDir)) < 10)
            {
                return;
            }
            //transform.forward = moveDir;
            canMove = true;
        }


        public void Update()
        {
            Vector3 motion = Vector3.zero;
            if (!characterController.isGrounded)
            {
                motion -= transform.up * numericComponent.GetAsFloat(NumericType.Speed) * Time.deltaTime;
            }
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
                    canMove = true;

                    moveDir = targetDirection;

                    //transform.forward = moveDir;

                    //characterController.Move(moveDir * GetParent<Unit>().GetComponent<NumericComponent>().GetAsFloat(NumericType.Speed) * Time.deltaTime);
                }
                else
                {
                    canMove = false;

                }

            }

            if (!canMove)
            {
                moveSpeed = 0.5f;
                animatorComponent.SetFloatValue("MoveSpeed", 0);
                return;
            }
            else
            {
                Quaternion quaDir = Quaternion.LookRotation(moveDir, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, quaDir, Time.deltaTime * 5);
                moveSpeed = Mathf.Clamp(moveSpeed += Time.deltaTime * 10, 0.5f, 1f);
                animatorComponent.SetFloatValue("MoveSpeed", moveSpeed);
            }
            if (!isInPlayerCtr)
                if (Vector3.Distance(transform.position, moveTarget) < 0.1f || Vector3.Dot(moveDir, moveTarget - transform.position) < 0)
                {
                    canMove = false;
                    return;
                }
            motion += moveDir * numericComponent.GetAsFloat(NumericType.Speed) * Time.deltaTime;
            characterController.Move(motion);


        }

        

    }
}
