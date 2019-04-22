using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class M2C_InterruptSkillHandler : AMHandler<M2C_InterruptSkill>
    {
        protected override void Run(ETModel.Session session, M2C_InterruptSkill message)
        {
            Unit unit = UnitComponent.Instance.Get(message.Id);

            unit.GetComponent<ActiveSkillComponent>().Interrupt( TypeOfInterruption.FromAll);


        }
    }
}
