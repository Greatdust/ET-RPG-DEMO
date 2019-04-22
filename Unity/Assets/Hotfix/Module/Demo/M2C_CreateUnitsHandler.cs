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
            
                unitData.groupIndex = (GroupIndex)unitInfo.GroupIndex;
                unitData.layerMask = (UnitLayerMask)unitInfo.LayerMask;
                unitData.unitLayer = (UnitLayer)unitInfo.UnitLayer;
                unitData.unitTag = (UnitTag)unitInfo.UnitTag;
                Unit unit = UnitFactory.Create(unitInfo.UnitId,1001, unitData);

                NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
                foreach (var v in unitInfo.UnitNumerics)
                {
                    numericComponent.Set((NumericType)v.Type, v.Value);
                }
                BattleEventHandler.LoadAssets(unit);


                Vector3 postion = new Vector3(unitInfo.Position.X, unitInfo.Position.Y, unitInfo.Position.Z);
                unit.GameObject.transform.forward = new Vector3(unitInfo.Dir.X, unitInfo.Dir.Y, unitInfo.Dir.Z);
                unit.Position = postion ;
                Dictionary<Type, IProperty> unitStateList = new Dictionary<Type, IProperty>();
                P_Position property_Position = new P_Position();
                property_Position.Value = postion ;// 防止掉下去
                unitStateList.Add(typeof(P_Position), property_Position);
                unit.GetComponent<UnitStateComponent>().Init(unitStateList);
                
            }
		}
	}
}
