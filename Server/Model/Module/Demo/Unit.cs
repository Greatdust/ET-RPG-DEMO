using Box2DSharp.Common;
using PF;
using System;
using UnityEngine;

namespace ETModel
{

	[ObjectSystem]
	public class UnitAwakeSystem : AwakeSystem<Unit>
	{
		public override void Awake(Unit self)
		{
			self.Awake();
		}
	}

	public sealed class Unit: Entity
	{

        private Property_Position property_Position;
        private Property_Rotation property_Rotation;
		
		public Vector3 Position {
            get
            {
                if (property_Position == null)
                {
                    UnitStateComponent unitState = GetComponent<UnitStateComponent>();
                    property_Position = unitState.unitProperty[typeof(Property_Position)] as Property_Position;
                }
                return property_Position.Get();
            }
            set
            {
                if (property_Position == null)
                {
                    UnitStateComponent unitState = GetComponent<UnitStateComponent>();
                    unitState.unitProperty[typeof(Property_Position)] = new Property_Position() { position = value };
                    property_Position = unitState.unitProperty[typeof(Property_Position)] as Property_Position;
                }
                property_Position.Set(value);
            }
        }

        public Quaternion Rotation
        {
            get 
            {
                if (property_Rotation == null)
                {
                    UnitStateComponent unitState = GetComponent<UnitStateComponent>();
                    property_Rotation = unitState.unitProperty[typeof(Property_Rotation)] as Property_Rotation;
                }
                return property_Rotation.Get();
            }
            set
            {
                if (property_Rotation == null)
                {
                    UnitStateComponent unitState = GetComponent<UnitStateComponent>();
                    unitState.unitProperty[typeof(Property_Rotation)] = new Property_Rotation() { value = value };
                    property_Rotation = unitState.unitProperty[typeof(Property_Rotation)] as Property_Rotation;
                }
                property_Rotation.Set(value);
            }
        }

        //这是被碰撞时触发的事件. 会抛出一个谁碰撞了它的回调
        public event Action<Unit, Vector3> OnCollisionEnterHandler;
        public event Action<Unit, Vector3> OnCollisionExitHandler;
        public event Action<Unit> OnCollisionStayHandler;

        public void OnCollisionEnter(Unit unit, Vector3 pos)
        {
            // Log.Debug("和unit  {0} 碰撞了,碰撞点在{1}", unit.UnitData.unitTag, pos);
            OnCollisionEnterHandler?.Invoke(unit, pos);
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


        //从Box2D的世界转换回3D空间
        public void UpdatePosFromPhysic(System.Numerics.Vector2 vector2)
        {
            property_Position.Set(vector2.ToVector3(Position.y));
        }

        //从Box2D的世界转换回3D空间
        public void UpdateRotFromPhysic(Rotation rotation)
        {
            property_Rotation.Set(rotation.ToRotation3D());
        }

        public UnitData UnitData
        {
            get; set;
        }


        public void Awake()
		{
			
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