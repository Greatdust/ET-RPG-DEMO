using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ETModel
{
    public class PhysicWorldComponentAwakeSystem : AwakeSystem<PhysicWorldComponent>
    {
        public override void Awake(PhysicWorldComponent self)
        {
            self.Awake();
        }
    }

    public class PhysicWorldComponent : Component
    {
        public World world;

        public static PhysicWorldComponent Instance;

        public void Awake()
        {
            Instance = this;
            world = new World(Vector2.Zero); //俯视角游戏,无重力
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            Instance = null;
        }
    }
}
