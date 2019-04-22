using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class M2C_GiveHealthHandler : AMHandler<M2C_GiveHealth>
    {
        protected override void Run(ETModel.Session session, M2C_GiveHealth message)
        {
         
            ETModel.Game.EventSystem.Run(ETModel.EventIdType.GiveHealth, message.Id, message.HealthValue);

        }
    }
}
