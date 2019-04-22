using ETModel;
using System.Collections.Generic;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_PlaySoundHandler : AMHandler<M2C_PlaySound>
	{
		protected override void Run(ETModel.Session session, M2C_PlaySound message)
		{
            BuffHandler_PlaySound.PlayAudio(message.AudioClipName, UnitComponent.Instance.Get(message.Id), message.PlaySpeed, message.Duration);
		}
	}
}
