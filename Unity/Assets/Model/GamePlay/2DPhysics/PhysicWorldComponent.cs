using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Numerics;

namespace ETModel
{
    [ObjectSystem]
    public class PhysicWorldComponentAwakeSystem : AwakeSystem<PhysicWorldComponent>
    {
        public override void Awake(PhysicWorldComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class PhysicWorldComponentFixedUpdateSystem : FixedUpdateSystem<PhysicWorldComponent>
    {
        public override void FixedUpdate(PhysicWorldComponent self)
        {
            self.FixedUpdate();
        }
    }

    public class PhysicWorldComponent : Component
    {
        public World world;

        public static PhysicWorldComponent Instance;

        public const int VelocityIteration = 10;
        public const int PositionIteration = 10;

        public void Awake()
        {
            Instance = this;
            world = new World(Vector2.Zero); //俯视角游戏,无重力
           // world.ContinuousPhysics = true;
        }

        public void FixedUpdate()
        {
            world.Step(EventSystem.FixedUpdateTime, VelocityIteration, PositionIteration);
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
