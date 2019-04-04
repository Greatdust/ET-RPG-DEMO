using Cinemachine;
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

	//[ObjectSystem]
	//public class CameraComponentLateUpdateSystem : LateUpdateSystem<CameraComponent>
	//{
	//	public override void LateUpdate(CameraComponent self)
	//	{
	//		self.LateUpdate();
	//	}
	//}

	public class CameraComponent : Component
	{
		// 战斗摄像机
		public Camera mainCamera;

        public Cinemachine.CinemachineFreeLook playerCam;

		public Unit Unit;

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

            Cinemachine.CinemachineCore.GetInputAxis = AxisInputDelegate;

            ReferenceCollector rc = mainCamera.GetComponent<ReferenceCollector>();
            playerCam = rc.Get<GameObject>("PlayerCam").GetComponent<CinemachineFreeLook>();
            Unit = GetParent<Unit>();
            playerCam.Follow = Unit.GameObject.transform;
            playerCam.LookAt = Unit.GameObject.GetComponentInChildren<CameraFollowHelper>().unitHead;

        }

        float AxisInputDelegate(string axisName)
        {
            if (!Input.GetMouseButton(1))
            {
                return 0;
            }
            return -Input.GetAxis(axisName);
        }

        public void LateUpdate()
		{
			// 摄像机每帧更新位置
			UpdatePosition();
		}

		private void UpdatePosition()
		{
			Vector3 cameraPos = this.mainCamera.transform.position;
			this.mainCamera.transform.position = new Vector3(this.Unit.Position.x, cameraPos.y, this.Unit.Position.z - 1);
		}
	}
}
