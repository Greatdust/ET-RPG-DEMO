using PF;
using UnityEngine;

namespace ETModel
{
	public enum UnitType
	{
		Hero,
		Npc
	}

	[ObjectSystem]
	public class UnitAwakeSystem : AwakeSystem<Unit, UnitType>
	{
		public override void Awake(Unit self, UnitType a)
		{
			self.Awake(a);
		}
	}

	public sealed class Unit: Entity
	{
		public UnitType UnitType { get; private set; }

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

		public void Awake(UnitType unitType)
		{
			this.UnitType = unitType;
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