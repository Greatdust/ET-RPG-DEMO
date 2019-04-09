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

    [ObjectSystem]
    public class UnitStateComponentFixedUpdateSystem : FixedUpdateSystem<UnitStateComponent>
    {
        public override void FixedUpdate(UnitStateComponent self)
        {
            self.FixedUpdate();
        }
    }

    //每个Unit身上挂的一个状态组件
    public class UnitStateComponent : Component
    {
        public Dictionary<int, UnitStateDelta> unitStatesDic; // 存储的是对应帧玩家状态的增量更新

        public Dictionary<Type, IProperty> unitProperty; //存储玩家现在帧的所有状态数据

        public Dictionary<Type, IProperty> pre_unitProperty; //存储玩家上一次确定帧时的所有状态数据

        private int preActualFrame;//上一个收到的实际帧
        private int remoteEstimatedFrame;// 估算帧
        private int remoteActualFrame; //实际帧

        private int packetsReceived = 1;

        private const int minDiffFrame = -20;//估算和真实帧最小差距
        private const int maxDiffFrame = 20;//估算和真实帧最大差距

        private const int largeDiffFrame = -15; // 落后多少帧就判断为极大落后

        private CommandSimulaterComponent simulaterComponent;

        private Unit unit;

        private bool haveInited;

        public ETCancellationTokenSource moveCancelTokenSource;

        public void Awake()
        {
            unitStatesDic = new Dictionary<int, UnitStateDelta>();
            unitProperty = new Dictionary<Type, IProperty>();
            simulaterComponent = Game.Scene.GetComponent<CommandSimulaterComponent>();
            AdjustRemoteEstimatedFrame();
            unit = GetParent<Unit>();
        }

        public void AdjustRemoteEstimatedFrame()
        {
            if (packetsReceived == 1)
            {
                remoteEstimatedFrame = remoteActualFrame;
            }
            else
            {
                int diffFrame = remoteActualFrame - remoteEstimatedFrame;
                if (diffFrame < minDiffFrame || diffFrame > maxDiffFrame)
                {
                    remoteEstimatedFrame = remoteActualFrame;
                }
            }
        }

        public void FixedUpdate()
        {
            if (unit == UnitComponent.Instance.MyUnit) return;//本机直接走预测/回滚路线

            if (!haveInited) return;
            if (unitStatesDic.Count == 0) return;

            if (remoteEstimatedFrame > remoteActualFrame) return;

            //AdjustRemoteEstimatedFrame();

            //float applyProgress = Mathf.Clamp01((float)(remoteEstimatedFrame - preActualFrame) / (remoteActualFrame - preActualFrame));//应用的进度,上一次实际帧和这一次实际帧之间
            
            //应用模拟的结果
            foreach (var v in unitStatesDic[remoteActualFrame].commandResults.Values)
            {
                switch (v)
                {
                    case CommandResult_Move result_Move:
                        unit.GetComponent<CharacterMoveComponent>().MoveAsync(result_Move.Path).Coroutine();
                        continue;
                }
            }
            remoteEstimatedFrame++;



        }

        //玩家刚开始的时候初始化所有的属性
        public void Init(Dictionary<Type, IProperty> unitProperty)
        {
            this.unitProperty = unitProperty;
            pre_unitProperty = new Dictionary<Type, IProperty>();
            foreach (var v in unitProperty)
            {
                pre_unitProperty.Add(v.Key, v.Value.GetCopy());
            }
            haveInited = true;
        }

        public void ReceivedPacket(int frame,ICommandResult commandResult)
        {
            UnitStateDelta unitState;
            if (unitStatesDic.ContainsKey(frame))
            {
                unitState = unitStatesDic[frame];
                unitState.commandResults[commandResult.GetType()] = commandResult;
            }
            else
            {

                unitState = new UnitStateDelta();
                unitState.frame = frame;
                unitState.commandResults.Add(commandResult.GetType(), commandResult);
                if (unitState.frame <= remoteActualFrame)
                {
                    //这是包顺序错了,目前处理是直接丢掉
                    return;
                }
                //重置上一个快照的属性
                foreach (var v in pre_unitProperty)
                {
                    switch (v.Value)
                    {
                        case Property_Position position:
                            position.Set(((Property_Position)unitProperty[v.Key]).Get());
                            break;
                    }
                }
                if (unitStatesDic.ContainsKey(preActualFrame))
                    //收到新的实际帧了,上上个实际帧的数据可以清理掉了
                    unitStatesDic.Remove(preActualFrame);

                packetsReceived++;
                preActualFrame = remoteActualFrame;
                remoteEstimatedFrame = remoteActualFrame = unitState.frame;
                unitStatesDic[remoteActualFrame] = unitState;
            }
            //等待上一帧的所有信息全收到了,再发上一帧的数据
            if (unit.GetComponent<CommandComponent>() != null)
            {
                GetParent<Unit>().GetComponent<CommandComponent>().simulateFrame = remoteActualFrame;
                unit.GetComponent<CommandComponent>().GetCommandResult(unitState);
            }
        }
         
        

    }
}
