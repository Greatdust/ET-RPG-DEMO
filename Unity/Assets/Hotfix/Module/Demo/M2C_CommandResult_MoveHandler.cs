using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_CommandResult_MoveHandler : AMHandler<M2C_InputResult_Move>
	{
		protected override void Run(ETModel.Session session, M2C_InputResult_Move message)
		{
			Unit unit = ETModel.Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
            if (message.PathList.Count == 0) return;
            List<Vector3> vector3s = new List<Vector3>();
            foreach (var v in message.PathList)
            {
                vector3s.Add(v.ToV3());
            }
            if (unit != ETModel.Game.Scene.GetComponent<UnitComponent>().MyUnit)
            {
                unit.GetComponent<CharacterMoveComponent>().MoveAsync(vector3s).Coroutine();
            }
            else
            {
                UnitStateDelta unitStateDelta = new UnitStateDelta();
                unitStateDelta.frame = message.Frame;
                CommandResult_Move commandResult_Move = CommandGCHelper.GetCommandResult<CommandResult_Move>();
                commandResult_Move.Path = vector3s;
                unitStateDelta.commandResult = commandResult_Move;
                unit.GetComponent<CommandComponent>().GetCommandResult(unitStateDelta);
            }

		}
	}
}
