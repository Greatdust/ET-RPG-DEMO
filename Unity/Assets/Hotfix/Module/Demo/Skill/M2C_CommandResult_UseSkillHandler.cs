using ETModel;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_CommandResult_UseSkill_DirHandler : AMHandler<InputResult_UseSkill_Dir>
	{
		protected override void Run(ETModel.Session session, InputResult_UseSkill_Dir message)
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
                if (!SkillHelper.tempData.ContainsKey((unit, message.PipelineSignal)))
                {
                    SkillHelper.tempData[(unit, message.PipelineSignal)] = new Dictionary<Type, IBufferValue>();
                }
                SkillHelper.tempData[(unit, message.PipelineSignal)][typeof(BufferValue_Dir)] = new BufferValue_Dir()
                {
                    dir = message.Dir.ToV3()
                };
                activeSkillComponent.Execute(message.SkillId).Coroutine();
            }

		}
	}
}
