using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
namespace ETHotfix
{

    [ObjectSystem]
    public class UnitStateMgrComponentFixedUpdateSystem : FixedUpdateSystem<UnitStateMgrComponent>
    {
        public override void FixedUpdate(UnitStateMgrComponent self)
        {
            self.FixedUpdate();
        }
    }

    public static class UnitStateMgrComponentSystem
    {
        public static void FixedUpdate(this UnitStateMgrComponent unitStateMgrComponent)
        {
            unitStateMgrComponent.currFrame++; //当前帧前进1
            foreach (var v in unitStateMgrComponent.unitStateComponents)
            {
                v.FixedUpdate();
            }
        }
    }
}