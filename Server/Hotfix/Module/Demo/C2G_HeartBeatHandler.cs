using System;
using System.Net;
using ETModel;

namespace ETHotfix
{
	[MessageHandler(AppType.Gate)]
	public class C2G_HeartBeatHandler : AMRpcHandler<C2R_HeartBeat, R2C_HeartBeat>
	{
        protected override void Run(Session session, C2R_HeartBeat message, Action<R2C_HeartBeat> reply)
        {
            RunAsync(session, message, reply);
        }

        protected void RunAsync(Session session, C2R_HeartBeat message, Action<R2C_HeartBeat> reply)
		{
            if (session == null) return;
            R2C_HeartBeat response = new R2C_HeartBeat();
			try
			{
               // session.GetComponent<HeartBeatComponent>().timing = TimeHelper.Now();
				reply(response);
			}
			catch (Exception e)
			{
				ReplyError(response, e, reply);
			}
		}
	}
}