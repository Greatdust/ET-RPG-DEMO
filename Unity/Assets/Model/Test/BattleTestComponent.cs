#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
 
            Unit v = UnitFactory.Create(IdGenerater.GenerateId(),1001);
            v.Position = new UnityEngine.Vector3(-10, 0, -10);
            UnitComponent.Instance.MyUnit = v;
            v.AddComponent<CameraComponent>();
            v.AddComponent<CommandComponent>();
        }
    }
}
#endif
