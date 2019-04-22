using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_CommandResult_HitEffectHandler : AMHandler<M2C_HitEffect>
	{
		protected override void Run(ETModel.Session session, M2C_HitEffect message)
		{
            BuffHandler_HitEffect.PlayHitEffect(message.HitObjId, message.Pos.ToV3(), message.Duration).Coroutine();
		}
	}
}
