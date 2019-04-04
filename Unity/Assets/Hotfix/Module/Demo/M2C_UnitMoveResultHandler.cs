using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[MessageHandler]
	public class M2C_UnitMoveResultHandler : AMHandler<Unit_PosAngle>
	{

        protected override void Run(ETModel.Session session, Unit_PosAngle message)
        {
            Log.Debug("接收到某单位的移动信息了 "+ message.Id.ToString());
            Unit unit = ETModel.Game.Scene.GetComponent<UnitComponent>().Get(message.Id);
            CharacterCtrComponent characterCtrComponent = unit.GetComponent<CharacterCtrComponent>();
            characterCtrComponent.MoveTo(new Vector3(message.MoveX, message.MoveY, message.MoveZ));
        }
    }
}
