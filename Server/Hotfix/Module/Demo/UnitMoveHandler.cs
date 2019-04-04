using ETModel;
using PF;
using UnityEngine;

namespace ETHotfix
{
	[ActorMessageHandler(AppType.Map)]
	public class UnitMoveHandler : AMActorLocationHandler<Unit, Unit_Move>
	{
		protected override void Run(Unit unit, Unit_Move message)
		{
			Vector3 target = new Vector3(message.MoveX, message.MoveY, message.MoveZ);
            Log.Info("Unit" + unit.Id + "  移动到了" + target.ToString());
            unit.Position = target;
            Unit_PosAngle unit_PosAngle = new Unit_PosAngle();
            unit_PosAngle.MoveX = message.MoveX;
            unit_PosAngle.MoveY = message.MoveY;
            unit_PosAngle.MoveZ = message.MoveZ;
            unit_PosAngle.EulerX = message.EulerX;
            unit_PosAngle.EulerY = message.EulerY;
            unit_PosAngle.EulerZ = message.EulerZ;
            unit_PosAngle.Id = unit.Id;
            Log.Info("传输的id是" + unit_PosAngle.Id.ToString());
            MessageHelper.Broadcast(unit_PosAngle);
      

        }
	}
}