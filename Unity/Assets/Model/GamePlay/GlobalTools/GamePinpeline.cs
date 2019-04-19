using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    //玩家正式进入游戏场景后的资源和配置加载 (非热更层)
    [Event(EventIdType.LoadAssets)]
    public class GamePinpeline_LoadAssets : AEvent
    {
        public override void Run()
        {
            Game.Scene.AddComponent<PathfindingComponent>();
            //如果是联网模式,客户端不做物理检测
            if (!GlobalConfigComponent.Instance.networkPlayMode)
            {
           
                Game.Scene.AddComponent<PhysicWorldComponent>();
                Game.Scene.AddComponent<PhysicCollisionListenerComponent>();
                Game.Scene.AddComponent<PStaticBodyMgrComponent>();
            }

            Game.Scene.AddComponent<BuffHandlerComponent>();

            Game.Scene.AddComponent<SkillConfigComponent>();
            Game.Scene.AddComponent<EffectCacheComponent>();
            Game.Scene.AddComponent<AudioCacheComponent>();
        }
    }





}
