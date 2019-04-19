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

        public Dictionary<Type, IProperty> unitProperty; //存储玩家当前所有状态数据,如果要做延迟补偿,那么就利用这个和上面对应帧的增量更新来弄.

        public const int maxFrameCount_SaveStateDelta = 120;//服务器最多存储玩家120帧的增量更新数据,即每隔60帧清理一次

        public bool collectInput = false;// 是否接收到了玩家的新输入.

        public CommandSimulaterComponent simulaterComponent;

        public Unit unit;

        public bool haveInited; //是否已经初始化玩家的所有属性

        public int preSendMsgFrame;

        public int currGetInputFrame;//最新的接收输入的是哪一帧,发送的时候就发送这一帧的数据
        public int preClearInputFrame;//最近一次清理输入数据的是哪一帧

        public ETCancellationTokenSource cancelToken;

        public InputResult_Move inputResult_Move = new InputResult_Move();
        public InputResult_UseSkill inputResult_UseSkill = new InputResult_UseSkill();


        public void Awake()
        {
            unitStatesDic = new Dictionary<int, UnitStateDelta>();
            unitProperty = new Dictionary<Type, IProperty>();
            simulaterComponent = Game.Scene.GetComponent<CommandSimulaterComponent>();
            unit = GetParent<Unit>();
        }

       
    }
}
