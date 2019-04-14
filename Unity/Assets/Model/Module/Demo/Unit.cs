using Box2DSharp.Common;
using PF;
using System;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace ETModel
{
    public enum UnitType
    {
        Static,
        Player,
        Monster,

    }

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
		}

        public void RemoveGameObject()
        {
            GameObject.Destroy(this.GameObject.GetComponent<ComponentView>());
            this.GameObject = null;
        }


        //这是被碰撞时触发的事件. 会抛出一个谁碰撞了它的回调
        public event Action<Unit> OnCollisionEnterHandler;
        public event Action<Unit> OnCollisionExitHandler;
        public event Action<Unit> OnCollisionStayHandler;

        public void OnCollisionEnter(Unit unit)
        {
            OnCollisionEnterHandler?.Invoke(unit);
        }
        public void OnCollisionStay(Unit unit)
        {
            OnCollisionStayHandler?.Invoke(unit);
        }
        public void OnCollisionExit(Unit unit)
        {
            OnCollisionExitHandler?.Invoke(unit);
        }

        public event Action<Vector3> OnPositionUpdate;
        public event Action<Quaternion> OnRotationUpdate;

        public UnitLayer UnitLayer = UnitLayer.Default;
        public UnitLayerMask LayerMask =  UnitLayerMask.ALL;
		
		public Vector3 Position
		{
			get
			{
				return GameObject.transform.position;
			}
			set
			{

				GameObject.transform.position = value;
                OnPositionUpdate?.Invoke(value);

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

        public UnitType UnitType
        {
            get;set;
        }

		public override void Dispose()
		{
			if (this.IsDisposed)
			{
				return;
			}

			base.Dispose();
		}
	}
}