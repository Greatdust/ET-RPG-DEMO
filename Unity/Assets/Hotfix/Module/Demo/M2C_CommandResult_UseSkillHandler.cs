using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_CommandResult_UseSkillHandler : AMHandler<InputResult_UseSkill>
	{
		protected override void Run(ETModel.Session session, InputResult_UseSkill message)
		{
			Unit unit = ETModel.Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
            ActiveSkillComponent activeSkillComponent = unit.GetComponent<ActiveSkillComponent>();
            if (unit == UnitComponent.Instance.MyUnit)
            {
                activeSkillComponent.tcs?.SetResult(message.Success);
                return;
            }
            if (message.Success)
            {
                activeSkillComponent.Execute(message.SkillId).Coroutine();
            }

		}
	}
}
