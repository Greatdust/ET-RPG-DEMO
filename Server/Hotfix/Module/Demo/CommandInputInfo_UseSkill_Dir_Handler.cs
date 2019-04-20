using ETModel;
using PF;
using UnityEngine;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class CommandInputInfo_UseSkill_Dir_Handler : AMActorLocationHandler<Unit, Input_UseSkill_Dir>
	{
		protected override void Run(Unit unit, Input_UseSkill_Dir message)
		{
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
            CommandInput_UseSkill commandInput_UseSkill = CommandGCHelper.GetCommandInput<CommandInput_UseSkill>();
            commandInput_UseSkill.skillId = message.SkillId;
            commandInput_UseSkill.pipelineSignal = message.PipelineSignal;
            commandInput_UseSkill.bufferValue = new BufferValue_Dir()
            {
                dir = message.AimDir.ToV3()
            };

            unitStateComponent.GetInput(message.Frame, commandInput_UseSkill);


        }
	}
}