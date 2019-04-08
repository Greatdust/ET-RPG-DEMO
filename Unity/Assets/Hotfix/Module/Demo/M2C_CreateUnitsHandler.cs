using ETModel;
using PF;
using System;
using System.Collections.Generic;
using Vector3 = UnityEngine.Vector3;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_CreateUnitsHandler : AMHandler<M2C_CreateUnits>
	{
		protected override void Run(ETModel.Session session, M2C_CreateUnits message)
		{	
			UnitComponent unitComponent = ETModel.Game.Scene.GetComponent<UnitComponent>();
			
			foreach (UnitInfo unitInfo in message.Units)
			{
				if (unitComponent.Get(unitInfo.UnitId) != null)
				{
					continue;
				}
				Unit unit = UnitFactory.Create(unitInfo.UnitId);

				unit.Position = new Vector3(unitInfo.Position.X,unitInfo.Position.Y,unitInfo.Position.Z);
                unit.GameObject.transform.forward = new Vector3(unitInfo.Dir.X, unitInfo.Dir.Y, unitInfo.Dir.Z);

                Dictionary<Type, IProperty> unitStateList = new Dictionary<Type, IProperty>();
                Property_Position property_Position = new Property_Position(unit);
                property_Position.Set(unit.Position);
                unitStateList.Add(typeof(Property_Position), property_Position);
                unit.GetComponent<UnitStateComponent>().Init(unitStateList);
                
            }
		}
	}
}
