using System;
using System.Collections.Generic;
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
                UnitData playerData = new UnitData();
                playerData.groupIndex = GroupIndex.Player;
                playerData.layerMask = UnitLayerMask.ALL;
                playerData.unitLayer = UnitLayer.Character;
                playerData.unitTag = UnitTag.Player;
                Unit unit = UnitFactory.Create(IdGenerater.GenerateId(),1001, playerData);
                await unit.AddComponent<MailBoxComponent>().AddLocation();
                unit.AddComponent<UnitGateComponent, long>(message.GateSessionId);


                unit.Position = new Vector3(-10, 0, -10);
           

                UnitStateComponent stateCom = unit.GetComponent<UnitStateComponent>();
     
                Game.Scene.GetComponent<UnitStateMgrComponent>().Add(stateCom);

                Game.Scene.GetComponent<UnitComponent>().Add(unit);
                response.UnitId = unit.Id;




                // 广播创建的unit
                M2C_CreateUnits createUnits = new M2C_CreateUnits();
                Unit[] units = Game.Scene.GetComponent<UnitComponent>().GetAll();
                foreach (Unit u in units)
                {
                    UnitInfo unitInfo = new UnitInfo();
                    UnitStateComponent unitStateComponent = u.GetComponent<UnitStateComponent>();
                    unitInfo.Position = u.Position.ToV3Info();
                    unitInfo.Dir = Vector3.forward.ToV3Info();
                    unitInfo.UnitId = u.Id;
                    unitInfo.GroupIndex = (int)u.UnitData.groupIndex;
                    unitInfo.LayerMask = (int)u.UnitData.layerMask;
                    unitInfo.UnitLayer = (int)u.UnitData.unitLayer;
                    unitInfo.UnitTag = (int)u.UnitData.unitTag;


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