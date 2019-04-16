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
                UnitData unitData = new UnitData();
                //TODO: 这个服务器要发
				Unit unit = UnitFactory.Create(unitInfo.UnitId,1001, unitData);

                Vector3 postion = new Vector3(unitInfo.Position.X, unitInfo.Position.Y, unitInfo.Position.Z);
                unit.GameObject.transform.forward = new Vector3(unitInfo.Dir.X, unitInfo.Dir.Y, unitInfo.Dir.Z);
                unit.Position = postion + new Vector3(0, 0.5f, 0);// 防止掉下去
                Dictionary<Type, IProperty> unitStateList = new Dictionary<Type, IProperty>();
                Property_Position property_Position = new Property_Position(unit);
                property_Position.Set(postion );// 防止掉下去
                unitStateList.Add(typeof(Property_Position), property_Position);
                unit.GetComponent<UnitStateComponent>().Init(unitStateList);
                
            }
		}
	}
}
