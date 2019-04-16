using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    //玩家正式进入游戏时的资源和配置加载 (非热更层)
    [Event(EventIdType.LoadAssets)]
    public class GamePinpeline_LoadAssets : AEvent
    {
        public override void Run()
        {
            Game.Scene.AddComponent<PathfindingComponent>();
            Game.Scene.AddComponent<PhysicWorldComponent>();
            Game.Scene.AddComponent<PhysicCollisionListenerComponent>();

            Game.Scene.AddComponent<GlobalVariableComponent>();
            Game.Scene.AddComponent<BuffHandlerComponent>();

            Game.Scene.AddComponent<SkillConfigComponent>();
            Game.Scene.AddComponent<EffectCacheComponent>();
            Game.Scene.AddComponent<AudioCacheComponent>();
        }
    }

    [Event(EventIdType.SceneChange)]
    public class GamePipeline_ChangeScene : AEvent<string,Action>
    {
        public override void Run(string sceneId, Action action)
        {
            SceneChangeHelper.SceneChange(sceneId, action, null);

        }
    }




}
