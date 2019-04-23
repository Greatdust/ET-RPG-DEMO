using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class PStaticBodyMgrComponentAwakeComponent : AwakeSystem<PStaticBodyMgrComponent>
    {
        public override void Awake(PStaticBodyMgrComponent self)
        {

            self.Awake();
        }
    }


    public class PStaticBodyMgrComponent : Component
    {
        public List<Unit> units;

        public const string configName = "MapData";

        public void Awake()
        {
            units = new List<Unit>();
            //////ToDo: 根据 mapId和配置文件,加载场景内所有StaticBody的数据
            //ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            //resourcesComponent.LoadBundle(configName.StringToAB());
            //GameObject prefab = resourcesComponent.GetAsset(configName.StringToAB(), configName) as GameObject;
            //var ta = prefab.Get<TextAsset>("1001");
            //var boxsData = Deserialize(ta.bytes);
            //UnitData unitData = new UnitData()
            //{
            //    groupIndex = GroupIndex.Default,
            //    layerMask = UnitLayerMask.ALL,
            //    unitLayer = UnitLayer.Default,
            //    unitTag = UnitTag.Static
            //};
            //foreach (var v in boxsData)
            //{
            //    Unit unit = UnitFactory.CreateStaticObj(new GameObject(), unitData);
            //    self.units.Add(unit);
            //}

            //客户端直接从场景里找到所有静态物体
            foreach (var v in GameObject.FindObjectsOfType<PBaseColliderHelper>())
            {
                if (v.bodyType != BodyType.StaticBody)
                    continue;
                UnitData unitData = new UnitData()
                {
                    groupIndex = GroupIndex.Default,
                    layerMask = UnitLayerMask.ALL,
                    unitLayer = UnitLayer.Default,
                    unitTag = UnitTag.Static
                };
                Unit unit = UnitFactory.CreateStaticObj(v.gameObject, unitData);
                units.Add(unit);
            }
            Log.Debug("加载了{0}个静态物体", units.Count);

        }

        public List<PBoxData> Deserialize(byte[] bytes)
        {
             List<PBoxData> pBoxDatas = MessagePack.MessagePackSerializer.Deserialize<List<PBoxData>>(bytes,
           MessagePack.Resolvers.ContractlessStandardResolver.Instance);
            return pBoxDatas;
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            units = null;
        }
    }
}
