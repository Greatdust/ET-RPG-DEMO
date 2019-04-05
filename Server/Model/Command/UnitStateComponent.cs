using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class UnitStateComponentAwakeSystem : AwakeSystem<UnitStateComponent>
    {
        public override void Awake(UnitStateComponent self)
        {
            self.Awake();
        }
    }

    //每个Unit身上挂的一个状态组件
    public class UnitStateComponent : Component
    {
        public Dictionary<int, UnitStateDelta> unitStatesDic; // 存储的是对应帧玩家状态的增量更新

        public Dictionary<Type, IProperty> unitProperty; //存储玩家上一次确定帧时的所有状态数据

        public int currFrame;
        public int preSendMsgFrame;

        public int preGetInputFrame;//上一个接收客户端输入的帧. 玩家真实状态会稳定到这里
        public int currGetInputFrame;//现在的接收客户端输入的帧. 利用

        public UnitStateDelta nextStateToSend;

        public CommandSimulaterComponent simulaterComponent;

        public Unit unit;

        public bool haveInited;

        public void Awake()
        {
            unitStatesDic = new Dictionary<int, UnitStateDelta>();
            unitProperty = new Dictionary<Type, IProperty>();
            simulaterComponent = Game.Scene.GetComponent<CommandSimulaterComponent>();
            unit = GetParent<Unit>();
            nextStateToSend = new UnitStateDelta();
        }

       
    }
}
