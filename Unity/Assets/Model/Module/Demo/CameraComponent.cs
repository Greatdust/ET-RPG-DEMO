using Cinemachine;
using System;
using UnityEngine;

namespace ETModel
{
	[ObjectSystem]
	public class CameraComponentAwakeSystem : AwakeSystem<CameraComponent>
	{
		public override void Awake(CameraComponent self)
		{
			self.Awake();
		}
	}

    [ObjectSystem]
    public class CameraComponentLateUpdateSystem : LateUpdateSystem<CameraComponent>
    {
        public override void LateUpdate(CameraComponent self)
        {
            self.LateUpdate();
        }
    }

    [ObjectSystem]
    public class CameraComponentFixedUpdateSystem : UpdateSystem<CameraComponent>
    {
        public override void Update(CameraComponent self)
        {
            self.Update();
        }
    }

    public class CameraComponent : Component
	{
		// 战斗摄像机
		public Camera mainCamera;

        public Cinemachine.CinemachineVirtualCamera playerCam;

		public Unit Unit;

        public Vector3 ClickPoint;

        public int mapMask;

        public CommandInput_Move commandInput;

        public bool incontrol = true;

        


        public Camera MainCamera
		{
			get
			{
				return this.mainCamera;
			}
		}

		public void Awake()
		{
			this.mainCamera = Camera.main;
            this.mapMask = LayerMask.GetMask("Map");
            commandInput = new CommandInput_Move();
            Cinemachine.CinemachineCore.GetInputAxis = AxisInputDelegate;

            ReferenceCollector rc = mainCamera.GetComponent<ReferenceCollector>();
            playerCam = rc.Get<GameObject>("PlayerCam").GetComponent<CinemachineVirtualCamera>();
            Unit = GetParent<Unit>();
            //playerCam.Follow = Unit.GameObject.transform;
            //playerCam.LookAt = Unit.GameObject.GetComponentInChildren<CameraFollowHelper>().unitHead;
            ResetPosition();
            // playerCam.Follow = Unit.GameObject.transform;
            mainCamera.GetComponent<MonoBehaviorEventHelpr>().appFocusEvent = (b) =>
            {
                incontrol = b;
            };

        }

        float AxisInputDelegate(string axisName)
        {
            if (!Input.GetMouseButton(1))
            {
                return 0;
            }
            return -Input.GetAxis(axisName);
        }

        public void Update()
        {
            if (!incontrol) return;
            if (Input.GetMouseButtonDown(1))
            {
 
                Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, 1000, this.mapMask))
                {
                    this.ClickPoint = hit.point;
                    commandInput.clickPos.x = this.ClickPoint.x;
                    commandInput.clickPos.y = this.ClickPoint.y;
                    commandInput.clickPos.z = this.ClickPoint.z;
                    this.Entity.GetComponent<CommandComponent>().CollectCommandInput(commandInput);
                }
            }
        }

        public void LateUpdate()
        {
            if (!incontrol) return;
            if (Input.GetKeyDown(KeyCode.Space))
            {
                ResetPosition();
            }
            // 摄像机每帧更新位置
            UpdatePosition();
        }

        private void ResetPosition()
        {
            Vector3 cameraPos = this.mainCamera.transform.position;
            this.mainCamera.transform.position = new Vector3(this.Unit.Position.x - 4, cameraPos.y, this.Unit.Position.z - 4);

        }

        //
		private void UpdatePosition()
		{
            float Width = Screen.width;
            float height = Screen.height;

            int xDelta = 0;
            int yDelta = 0;


            if (Input.mousePosition.x < Width * 0.1f)
            {
                xDelta += 1;
                yDelta -= 1;
 
            }
            if (Input.mousePosition.x > Width * 0.9f)
            {
                xDelta -= 1;
                yDelta += 1;
            }
            if (Input.mousePosition.y < height * 0.1f)
            {
                xDelta -= 1;
                yDelta -= 1;
            }
            if (Input.mousePosition.y > height * 0.9f)
            {
                xDelta += 1;
                yDelta += 1;

            }




            if (xDelta == 0 && yDelta == 0)
            {
                return;
            }




            Vector3 move = new Vector3( yDelta , 0, xDelta )  * 10 * Time.deltaTime;

            playerCam.transform.position += move;

        }
	}
}
