using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_DisposeEmitObjHandler : AMHandler<M2C_DisposeEmitObj>
	{
        protected override void Run(ETModel.Session session, M2C_DisposeEmitObj message)
        {
            Unit unit = ETModel.Game.Scene.GetComponent<EmitObjUnitComponent>().Get(message.Id);
            if (unit != null)
            {
                unit.GetComponent<EmitObjMoveComponent>().OnEnd((UnitComponent.Instance.Get(message.UnitId), message.Pos.ToV3()));
            }
		}
	}
}
