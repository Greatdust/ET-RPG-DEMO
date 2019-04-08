using System;
using System.Collections.Generic;
using BulletUnity;
using ETModel;
using PF;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler(AppType.Map)]
    public class G2M_CreateUnitHandler : AMRpcHandler<G2M_CreateUnit, M2G_CreateUnit>
    {
        protected override void Run(Session session, G2M_CreateUnit message, Action<M2G_CreateUnit> reply)
        {
            RunAsync(session, message, reply).Coroutine();
        }

        protected async ETVoid RunAsync(Session session, G2M_CreateUnit message, Action<M2G_CreateUnit> reply)
        {
            M2G_CreateUnit response = new M2G_CreateUnit();
            try
            {
                Unit unit = ComponentFactory.CreateWithId<Unit>(IdGenerater.GenerateId());
                await unit.AddComponent<MailBoxComponent>().AddLocation();
                unit.AddComponent<UnitGateComponent, long>(message.GateSessionId);
                unit.AddComponent<NumericComponent>();

                Dictionary<Type, IProperty> unitProperty = new Dictionary<Type, IProperty>();
                Property_Position property_Position = new Property_Position();
                property_Position.Set(new Vector3(-10, 2f, -10));
                Property_Rotation property_Rotation = new Property_Rotation();
                property_Rotation.Set(Quaternion.identity);
                unitProperty.Add(typeof(Property_Position), property_Position);
                unitProperty.Add(typeof(Property_Rotation), property_Rotation);

                UnitStateComponent stateCom = unit.AddComponent<UnitStateComponent>();
                stateCom.Init(unitProperty);
                Game.Scene.GetComponent<UnitStateMgrComponent>().Add(stateCom);

                BBoxShape bBoxShape = unit.AddComponent<BBoxShape>();
                bBoxShape.Extents = new Vector3(0.2f, 0.9f, 0.2f);
                unit.AddComponent<BCharacterController,BCollisionShape>(bBoxShape);
                unit.AddComponent<CharacterCtrComponent>();
                Game.Scene.GetComponent<UnitComponent>().Add(unit);
                response.UnitId = unit.Id;




                // 广播创建的unit
                M2C_CreateUnits createUnits = new M2C_CreateUnits();
                Unit[] units = Game.Scene.GetComponent<UnitComponent>().GetAll();
                foreach (Unit u in units)
                {
                    UnitInfo unitInfo = new UnitInfo();
                    UnitStateComponent unitStateComponent = u.GetComponent<UnitStateComponent>();
                    Vector3 pos = ((Property_Position)unitStateComponent.unitProperty[typeof(Property_Position)]).Get();
                    unitInfo.Position = new Vector3Info() { X = pos.x, Y = pos.y, Z = pos.z };
                    unitInfo.Dir = new Vector3Info() { X = 0, Y = 0, Z = 1 };
                    unitInfo.UnitId = u.Id;
                    
                    createUnits.Units.Add(unitInfo);
                }
                MessageHelper.Broadcast(createUnits);


                reply(response);
            }
            catch (Exception e)
            {
                ReplyError(response, e, reply);
            }
        }
    }
} 