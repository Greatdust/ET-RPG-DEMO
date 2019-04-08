using System;
using BulletUnity;
using CommandLine;
using ETModel;

namespace ETHotfix
{
    [ObjectSystem]
    public class PhysicWorldComponentSystem : AwakeSystem<PhysicWorldComponent, int>
    {
        public override void Awake(PhysicWorldComponent self, int worldId)
        {
            PhysicWorldsConfig physicsConfig = (PhysicWorldsConfig)Game.Scene.GetComponent<ConfigComponent>().Get(typeof(PhysicWorldsConfig), worldId);

            self.physicsWorld = Game.Scene.AddComponent<BPhysicsWorld>();

            if (!string.IsNullOrEmpty(physicsConfig.TerrainDataPath))
            {
                //TODO : 加载地形数据
            }
            if (!string.IsNullOrEmpty(physicsConfig.StaticObjs_Box))
            {
                self.staticObjs_box = BulletDataLoadTool.LoadStaticObjects_BoxShape(physicsConfig.StaticObjs_Box);
            }
        }
    }


}
