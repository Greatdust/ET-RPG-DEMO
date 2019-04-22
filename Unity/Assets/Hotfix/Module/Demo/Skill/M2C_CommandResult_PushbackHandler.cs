using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class M2C_CommandResult_PushbackHandler : AMHandler<M2C_Pushback>
    {
        protected override async void Run(ETModel.Session session, M2C_Pushback message)
        {
            Unit unit = UnitComponent.Instance.Get(message.Id);

            CharacterMoveComponent characterMoveComponent = unit.GetComponent<CharacterMoveComponent>();
            float moveSpeed = Vector3.Distance(unit.Position, message.MoveTarget.ToV3()) / message.Time;
            CharacterStateComponent characterStateComponent = unit.GetComponent<CharacterStateComponent>();
            characterStateComponent.Set(SpecialStateType.NotInControl, true);
            await characterMoveComponent.PushBackedTo(message.MoveTarget.ToV3(), moveSpeed);
            characterStateComponent.Set(SpecialStateType.NotInControl, false);

        }
    }
}
