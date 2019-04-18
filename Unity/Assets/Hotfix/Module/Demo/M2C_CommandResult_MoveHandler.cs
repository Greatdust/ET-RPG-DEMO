using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_CommandResult_MoveHandler : AMHandler<InputResult_Move>
	{
		protected override void Run(ETModel.Session session, InputResult_Move message)
		{
			Unit unit = ETModel.Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
            CommandResult_Move commandResult_Move = CommandGCHelper.GetCommandResult<CommandResult_Move>();
            commandResult_Move.Path = new List<Vector3>();
            for (int i = 0; i < message.PathList.Count; i++)
            {
                commandResult_Move.Path.Add(new Vector3(message.PathList[i].X, message.PathList[i].Y, message.PathList[i].Z));
            }
            unitStateComponent.ReceivedPacket(message.Frame, commandResult_Move);

		}
	}
}
