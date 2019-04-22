using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
    [MessageHandler]
    public class M2C_GiveDamageHandler : AMHandler<M2C_GiveDamage>
    {
        protected override void Run(ETModel.Session session, M2C_GiveDamage message)
        {
            GameCalNumericTool.DamageData[] damageDatas = new GameCalNumericTool.DamageData[message.DamageDatas.Count];
            for (int i = 0; i < damageDatas.Length; i++)
            {
                damageDatas[i] = new GameCalNumericTool.DamageData();
                damageDatas[i].damageType = (GameCalNumericTool.DamageType)message.DamageDatas[i].DamageType;
                damageDatas[i].damageValue = message.DamageDatas[i].DamageValue;
            }
            ETModel.Game.EventSystem.Run(ETModel.EventIdType.GiveDamage, message.Id, damageDatas);

        }
    }
}
