using ETModel;
using PF;
using UnityEngine;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class CommandInputInfo_MoveHandler : AMActorLocationHandler<Unit, CommandInputInfo_Move>
	{
		protected override void Run(Unit unit, CommandInputInfo_Move message)
		{
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
            
            CommandInput_Move commandInput_Move = new CommandInput_Move();
            commandInput_Move.moveDir = new Vector3(message.MoveDir.X, message.MoveDir.Y, message.MoveDir.Z);
            unitStateComponent.GetInput(message.Frame, commandInput_Move);


        }
	}
}