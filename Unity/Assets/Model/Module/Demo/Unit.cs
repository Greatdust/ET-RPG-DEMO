using Box2DSharp.Common;
using PF;
using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace ETModel
{
    [ObjectSystem]
	public class UnitAwakeSystem : AwakeSystem<Unit, GameObject>
	{
		public override void Awake(Unit self, GameObject gameObject)
		{
			self.Awake(gameObject);
		}
	}
	
	[HideInHierarchy]
	public sealed class Unit: Entity
	{
		public void Awake(GameObject gameObject)
		{
			this.GameObject = gameObject;
			this.GameObject.AddComponent<ComponentView>().Component = this;
            this.GameObject.AddComponent<UnitGameObjectHelper>().UnitId = this.Id;
		}

        public void RemoveGameObject()
        {
            GameObject.Destroy(this.GameObject.GetComponent<ComponentView>());
            Position = hidePos;
            this.GameObject = null;
        }

        private readonly Vector3 hidePos = new Vector3(0, 5000, 0);
        //这是被碰撞时触发的事件. 会抛出一个谁碰撞了它的回调
        public event Action<Unit,Vector3> OnCollisionEnterHandler;
        public event Action<Unit, Vector3> OnCollisionExitHandler;
        public event Action<Unit> OnCollisionStayHandler;

        public void OnCollisionEnter(Unit unit, Vector3 pos)
        {
            OnCollisionEnterHandler?.Invoke(unit,pos);
        }
        public void OnCollisionStay(Unit unit)
        {
            OnCollisionStayHandler?.Invoke(unit);
        }
        public void OnCollisionExit(Unit unit, Vector3 pos)
        {
            OnCollisionExitHandler?.Invoke(unit, pos);
        }

        public event Action<Vector3> OnPositionUpdate;
        public event Action<Quaternion> OnRotationUpdate;

        public float HalfHeight;//半高,主要用以模拟3D的检测
        public float OffsetY;//Y轴位置偏移量,主要用以模拟3D的检测
		
		public Vector3 Position
		{
			get
			{
                if (GameObject != null)
                    return GameObject.transform.position;
                else
                {
                    return Vector3.zero;
                }
			}
			set
			{
                if (GameObject != null)
                {
                    GameObject.transform.position = value;
                    OnPositionUpdate?.Invoke(value);
                }

            }
		}

        //从Box2D的世界转换回3D空间
        public void UpdatePosFromPhysic(System.Numerics.Vector2 vector2)
        {
            GameObject.transform.position = vector2.ToVector3(GameObject.transform.position.y);
        }

		public Quaternion Rotation
		{
			get
			{
				return GameObject.transform.rotation;
			}
			set
			{
				GameObject.transform.rotation = value;
                OnRotationUpdate?.Invoke(value);

            }
		}

        //从Box2D的世界转换回3D空间
        public void UpdateRotFromPhysic(Rotation rotation)
        {
            GameObject.transform.rotation = rotation.ToRotation3D();
        }

        public UnitData UnitData
        {
            get;set;
        }

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}
            if (OnCollisionEnterHandler != null)
            {
                var enterDelegates = OnCollisionEnterHandler.GetInvocationList();
                for (int i = 0; i < enterDelegates.Length; i++)
                {
                    OnCollisionEnterHandler -= enterDelegates[i] as Action<Unit,Vector3>;
                }
            }
            if (OnCollisionExitHandler != null)
            {
                var exitDelegates = OnCollisionExitHandler.GetInvocationList();
                for (int i = 0; i < exitDelegates.Length; i++)
                {
                    OnCollisionExitHandler -= exitDelegates[i] as Action<Unit, Vector3>;
                }
            }
            if (OnCollisionStayHandler != null)
            {
                var stayDelegates = OnCollisionStayHandler.GetInvocationList();
                for (int i = 0; i < stayDelegates.Length; i++)
                {
                    OnCollisionStayHandler -= stayDelegates[i] as Action<Unit>;
                }
            }
            Position = hidePos;
            base.Dispose();
		}
	}
}