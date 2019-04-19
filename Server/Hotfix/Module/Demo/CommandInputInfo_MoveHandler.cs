using ETModel;
using PF;
using UnityEngine;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class CommandInputInfo_MoveHandler : AMActorLocationHandler<Unit, Input_Move>
	{
		protected override void Run(Unit unit, Input_Move message)
		{
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
            
            CommandInput_Move commandInput_Move = new CommandInput_Move();
            commandInput_Move.clickPos = new Vector3(message.AimPos.X, message.AimPos.Y, message.AimPos.Z);
            unitStateComponent.GetInput(message.Frame, commandInput_Move);


        }
	}
}