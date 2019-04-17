using ETModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace ETHotfix
{
    [ObjectSystem]
    public class PStaticBodyMgrComponentAwakeSystem : AwakeSystem<PStaticBodyMgrComponent,int>
    {
        public override void Awake(PStaticBodyMgrComponent self,int mapId)
        {
            self.Awake(mapId);
        }
    }

    public static class PStaticBodyMgrComponentSystem
    {
        public static void Awake(this PStaticBodyMgrComponent self, int mapId)
        {
            self.units = new List<Unit>();
            //ToDo: 根据 mapId和配置文件,加载场景内所有StaticBody的数据
            PhysicWorldsConfig worldsConfig = ETModel.Game.Scene.GetComponent<ETModel.ConfigComponent>().Get(typeof(PhysicWorldsConfig), mapId) as PhysicWorldsConfig;


            var boxsData = Deserialize(worldsConfig.StaticObjs_Box);

            UnitData unitData = new UnitData()
            {
                groupIndex = GroupIndex.Default,
                layerMask = UnitLayerMask.ALL,
                unitLayer = UnitLayer.Default,
                unitTag = UnitTag.Static
            };
            foreach (var v in boxsData)
            {
                Unit unit = UnitFactory.CreateStaticObj_Box(unitData, v);
                self.units.Add(unit);
            }


            Log.Info(string.Format("加载了{0}个静态物体", self.units.Count));

        }
        public static List<PBoxData> Deserialize(string path)
        {
            FileStream file = File.OpenRead(path);
            BinaryFormatter bf = new BinaryFormatter();
            //序列化
            List<PBoxData> pBoxDatas = bf.Deserialize(file) as List<PBoxData>;
            file.Dispose();
            file.Close();
            return pBoxDatas;

        }

    }
}
