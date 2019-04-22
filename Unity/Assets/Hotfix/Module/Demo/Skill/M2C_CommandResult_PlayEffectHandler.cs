using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class M2C_CommandResult_PlayEffectHandler : AMHandler<M2C_PlayEffect>
    {
        protected override void Run(ETModel.Session session, M2C_PlayEffect message)
        {
            Unit unit = UnitComponent.Instance.Get(message.Id);

            BuffHandler_PlayEffect.AddEffect(unit, message.BuffSignal, message.EffectObjId, message.LockTarget, message.Pos.ToV3(),
             message.CanBeInterupt, unit.GetComponent<ActiveSkillComponent>().cancelToken.Token, message.Duration,null).Coroutine();


        }
    }
}
