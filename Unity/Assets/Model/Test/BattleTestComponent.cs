#if UNITY_EDITOR
using Box2DSharp.Collision.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class BattleTestComponentAwakeSystem : AwakeSystem<BattleTestComponent>
    {
        public override void Awake(BattleTestComponent self)
        {
            self.Awake();
        }
    }


    public class BattleTestComponent :Component
    {
        public void Awake()
        {
            Game.Scene.GetComponent<GlobalConfigComponent>().networkPlayMode = false;
            Game.EventSystem.Run(EventIdType.LoadAssets);
            {
                UnitData playerData = new UnitData();
                playerData.groupIndex = GroupIndex.Player;
                playerData.layerMask = UnitLayerMask.ALL;
                playerData.unitLayer = UnitLayer.Character;
                playerData.unitTag = UnitTag.Player;
                //创建主角
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, playerData);
                v.Position = new UnityEngine.Vector3(-10, 0, -10);
                UnitComponent.Instance.MyUnit = v;
                v.AddComponent<CameraComponent>();
                v.AddComponent<CommandComponent>();
                var input = v.AddComponent<InputComponent>();
                var list = v.GetComponent<ActiveSkillComponent>().skillList.Keys.ToArray();
                input.AddSkillToHotKey("Q", list[0]);
                input.AddSkillToHotKey("W", list[1]);
                input.AddSkillToHotKey("E", list[2]);
                BattleEventHandler.LoadAssets(v);
            }
            UnitData monsterData = new UnitData();
            monsterData.groupIndex = GroupIndex.Monster;
            monsterData.layerMask = UnitLayerMask.ALL;
            monsterData.unitLayer = UnitLayer.Character;
            monsterData.unitTag = UnitTag.Monster;
            {
                //创建怪物
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, monsterData);
                v.Position = new UnityEngine.Vector3(4.2f, 4, -15);
                v.Rotation = UnityEngine.Quaternion.LookRotation(v.Position - UnitComponent.Instance.MyUnit.Position, Vector3.up);
                //v.AddComponent<PDynamicBodyComponent, Shape>(new CircleShape() { Radius = 0.5f });

                BattleEventHandler.LoadAssets(v);
            }
            {
                //创建怪物
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, monsterData);
                v.Position = new UnityEngine.Vector3(-10, 0, 11);
                v.Rotation = UnityEngine.Quaternion.LookRotation(v.Position - UnitComponent.Instance.MyUnit.Position, Vector3.up);
                //v.AddComponent<PDynamicBodyComponent, Shape>(new CircleShape() { Radius = 0.5f });

                BattleEventHandler.LoadAssets(v);
            }
            {
                //创建怪物
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, monsterData);
                v.Position = new UnityEngine.Vector3(-10, 0, 15);
                v.Rotation = UnityEngine.Quaternion.LookRotation(v.Position - UnitComponent.Instance.MyUnit.Position, Vector3.up);
                //v.AddComponent<PDynamicBodyComponent, Shape>(new CircleShape() { Radius = 0.5f });

                BattleEventHandler.LoadAssets(v);
            }
        }
    }
}
#endif
