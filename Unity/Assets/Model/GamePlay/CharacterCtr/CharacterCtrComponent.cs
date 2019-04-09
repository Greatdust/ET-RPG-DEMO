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

        public CharacterController bCharacterController;

        public AnimatorComponent animatorComponent;

        public CameraComponent cameraComponent;

        private Transform transform;

        public bool isInPlayerCtr;

        public float sendMsgInterval = 0.2f;
        public float timing;

        private CommandInput_Move input_Move;

        private NumericComponent numericComponent;

        private float moveSpeed = 0;
        public UnitStateComponent unitState;
        public void Awake()
        {
            transform = GetParent<Unit>().GameObject.transform;
            animatorComponent = GetParent<Unit>().GetComponent<AnimatorComponent>();
            timing = sendMsgInterval;
            input_Move = new CommandInput_Move();
            numericComponent = GetParent<Unit>().GetComponent<NumericComponent>();
            bCharacterController = GetParent<Unit>().GameObject.GetComponent<CharacterController>();
            unitState = GetParent<Unit>().GetComponent<UnitStateComponent>();
        }

        public void MoveTo(Vector3 aim)
        {
            moveTarget = aim;
            Vector3 distance = new Vector3(moveTarget.x - transform.position.x, 0, moveTarget.z - transform.position.z);
            float length = distance.magnitude;
            if (length < 0.05f)
            {
                // bCharacterController.SetPosition(aim);
                transform.position = aim;
                return;
            }
            moveSpeed = length / EventSystem.FixedUpdateTime;
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
                    input_Move.moveDir = new Vector3(targetDirection.x, 0, targetDirection.z); // 暂时不发送Y轴可能产生的移动信息
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
            //Log.Debug("移动方向"+ moveDir.ToString());
            Quaternion quaDir = Quaternion.LookRotation(moveDir, Vector3.up);
            //bCharacterController.ChangeRotation(quaDir);
            transform.rotation = quaDir;
           // bCharacterController.ChangeRotation(Quaternion.Slerp(transform.rotation, quaDir, Time.deltaTime * 5));
            if (Vector3.Distance(transform.position, moveTarget) < 0.05f || Vector3.Dot(moveDir, moveTarget - transform.position) < 0)
            {
                canMove = false;
                //bCharacterController.SetPosition(moveTarget);
                transform.position = moveTarget;
                return;
            };

            //因为未知的原因,Bullet的角色移动,速度比正常情况下要慢 .这个速度不影响最终位置
            bCharacterController.Move(moveDir * EventSystem.FixedUpdateTime * moveSpeed );

        }


        

    }
}
