using System;
using System.Linq;
using ETModel;

namespace ETHotfix
{
    public static class MapHelper
    {
        public static async ETVoid EnterMapAsync()
        {
            try
            {
                // 加载场景资源
                await ETModel.Game.Scene.GetComponent<ResourcesComponent>().LoadBundleAsync("map.unity3d");
                // 切换到map场景
                using (SceneChangeComponent sceneChangeComponent = ETModel.Game.Scene.AddComponent<SceneChangeComponent>())
                {
                    await sceneChangeComponent.ChangeSceneAsync(SceneType.Map);
                }
                ETModel.Game.EventSystem.Run(ETModel.EventIdType.LoadAssets);
                // 加载Unit资源
                //ResourcesComponent resourcesComponent = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
              //  await resourcesComponent.LoadBundleAsync($"unit.unity3d");


                G2C_EnterMap g2CEnterMap = await ETModel.SessionComponent.Instance.Session.Call(new C2G_EnterMap()) as G2C_EnterMap;
                PlayerComponent.Instance.MyPlayer.UnitId = g2CEnterMap.UnitId;

                foreach (var v in UnitComponent.Instance.GetAll())
                {

                    if (v.Id == PlayerComponent.Instance.MyPlayer.UnitId)
                    {
                        UnitComponent.Instance.MyUnit = v;
                        v.AddComponent<CameraComponent>();
                        v.AddComponent<CommandComponent>().simulateFrame = g2CEnterMap.Frame;
                        Log.Debug("收到的当前帧为" + g2CEnterMap.Frame);
                        var list = v.GetComponent<ActiveSkillComponent>().skillList.Keys.ToArray();
                        var input = v.AddComponent<InputComponent>();
                        input.AddSkillToHotKey("Q", list[0]);
                        //input.AddSkillToHotKey("W", list[1]); // 暂时没做使用技能时,位置输入和目标输入的数据传输.
                        //input.AddSkillToHotKey("E", list[2]);
                        break;
                    }

                }


                //Game.Scene.AddComponent<OperaComponent>();

                Game.EventSystem.Run(EventIdType.EnterMapFinish);
            }
            catch (Exception e)
            {
                Log.Error(e);
            }	
        }
    }
}