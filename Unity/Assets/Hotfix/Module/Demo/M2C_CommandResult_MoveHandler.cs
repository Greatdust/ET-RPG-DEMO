using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_CommandResult_MoveHandler : AMHandler<CommandResultInfo_Move>
	{
		protected override void Run(ETModel.Session session, CommandResultInfo_Move message)
		{
			Unit unit = ETModel.Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
            CommandResult_Move commandResult_Move = CommandGCHelper.GetCommandResult<CommandResult_Move>();
            commandResult_Move.postion = new Vector3(message.Pos.X, message.Pos.Y, message.Pos.Z);
            unitStateComponent.ReceivedPacket(message.Frame, commandResult_Move);
		}
	}
}
