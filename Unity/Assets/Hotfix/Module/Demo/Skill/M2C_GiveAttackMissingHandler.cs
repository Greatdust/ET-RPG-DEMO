using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class M2C_GiveAttackMissingHandler : AMHandler<M2C_GiveAttackMissing>
    {
        protected override void Run(ETModel.Session session, M2C_GiveAttackMissing message)
        {

            ETModel.Game.EventSystem.Run(ETModel.EventIdType.AttackMissing, message.Id);

        }
    }
}
