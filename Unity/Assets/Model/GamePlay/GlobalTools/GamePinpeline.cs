using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{

    [Event(EventIdType.LoadAssets)]
    public class GamePinpeline_LoadAssets : AEvent
    {
        public override void Run()
        {
            Game.Scene.AddComponent<GlobalVariableComponent>();
            Game.Scene.AddComponent<BuffHandlerComponent>();

            Game.Scene.AddComponent<SkillConfigComponent>();

            Game.Scene.AddComponent<ItemConfigComponent>();
            Game.Scene.AddComponent<EquipConfigComponent>();

            Game.Scene.AddComponent<EffectCacheComponent>();
            Game.Scene.AddComponent<AudioCacheComponent>();

            //创建主角的Unit (管理战斗相关的)

            //创建主角的化身,Player(管理非战斗相关的)

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
