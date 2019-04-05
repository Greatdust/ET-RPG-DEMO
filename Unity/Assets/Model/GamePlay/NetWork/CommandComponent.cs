using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class CommandComponentAwakeSystem : AwakeSystem<CommandComponent>
    {
        public override void Awake(CommandComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class CommandComponentFixedUpdateSystem : FixedUpdateSystem<CommandComponent>
    {
        public override void FixedUpdate(CommandComponent self)
        {
            self.FixedUpdate();
        }
    }

    public class CommandComponent : Component
    {
        public Dictionary<int, List<Command>> cacheCommands = new Dictionary<int, List<Command>>();
        public Dictionary<Type,Command> currCommands;
        private bool collectedNewInput;

        private Unit unit;
        private CommandSimulaterComponent simulaterComponent;

        private CommandInputInfo_Move inputInfo_Move = new CommandInputInfo_Move();

        public int currFrame;

        public int preActualFrame; //上一次收到服务器发来的确定帧

        private const int maxDiffFrame = 120;//当前帧和上一次服务器发来的帧之间差距超过多少,就不模拟了


        public UnitStateComponent unitState;

        public void Awake()
        {
            currCommands = new Dictionary<Type, Command>();
            unit = GetParent<Unit>();
            if (unit == null)
                Log.Debug("获取不到Unit");
            simulaterComponent = Game.Scene.GetComponent<CommandSimulaterComponent>();
            unitState = unit.GetComponent<UnitStateComponent>();
        }

        public void FixedUpdate()
        {
            //先上传这一帧的操作,同时缓存操作,然后客户端自己模拟操作的结果
            currFrame++;
            if (collectedNewInput)
            {
                collectedNewInput = false;
                cacheCommands[currFrame] = new List<Command>();
                cacheCommands[currFrame].AddRange(currCommands.Values);
                currCommands.Clear();
                foreach (var v in cacheCommands[currFrame])
                {

                    switch (v.commandInput)
                    {
                        case CommandInput_Move input_Move:

                            CommandResult_Move result_Move = simulaterComponent.commandSimulaters[input_Move.GetType()].Simulate(input_Move, unit) as CommandResult_Move;
                            v.commandResult = result_Move;

                            //每秒最多发送60次
                            inputInfo_Move.Frame = currFrame;
                            inputInfo_Move.MoveDir = new Vector3Info() { X = input_Move.moveDir.x, Y = input_Move.moveDir.y, Z = input_Move.moveDir.z };
                            ETModel.Game.Scene.GetComponent<SessionComponent>().Session.Send(inputInfo_Move);

                            //再预测这一帧的结果
                            Property_Position property_Position = unitState.unitProperty[typeof(Property_Position)] as Property_Position;
                            property_Position.Set(result_Move.postion);
                            unit.GetComponent<CharacterCtrComponent>().MoveTo(result_Move.postion);
                            break;
                    }
                }
            }



        }

        public void CollectCommandInput(ICommandInput input)
        {
            collectedNewInput = true;
            Command command = new Command();
            command.commandInput = input;
            currCommands[input.GetType()] = command;

        }

        public void GetCommandResult(UnitStateDelta unitStateDelta)
        {
            //移除上一次的确定帧和接收到的这一帧之间的缓存指令
            for (int i = preActualFrame; i < unitStateDelta.frame; i++)
            {
                if (cacheCommands.ContainsKey(i))
                {
                    cacheCommands.Remove(i);
                }
            }
            bool needRecal = false;
            if (cacheCommands.ContainsKey(unitStateDelta.frame))
                foreach (var v in cacheCommands[unitStateDelta.frame])
                {
                    switch (v.commandResult)
                    {
                        case CommandResult_Move simulateMove:
                            CommandResult_Move actualMove = unitStateDelta.commandResults[v.commandResult.GetType()] as CommandResult_Move;
                            if (Vector3.Distance(simulateMove.postion, actualMove.postion) < 0.1f)
                            {
                                continue;
                            }
                            else
                            {
                                simulateMove.postion = actualMove.postion;
                                needRecal = true;
                            }
                            break;
                    }
                }
            preActualFrame = unitStateDelta.frame;
            if (needRecal)
            {
                ReCal(unitStateDelta.frame);
            }

        }

        //从哪一帧开始重新计算
        public void ReCal(int startFrame)
        {
            Property_Position property_Position = unitState.unitProperty[typeof(Property_Position)] as Property_Position;

            bool init = false;

            for (int i = startFrame; i <= currFrame; i++)
            {
                if (cacheCommands.ContainsKey(i))
                    foreach (var v in cacheCommands[i])
                    {

                        switch (v.commandInput)
                        {
                            case CommandInput_Move input_Move:
                                if (!init)
                                {
                                    CommandResult_Move commandResult = v.commandResult as CommandResult_Move;
                                    property_Position.Set(commandResult.postion);
                                }
                                else
                                {
                                    CommandResult_Move result_Move = simulaterComponent.commandSimulaters[input_Move.GetType()].Simulate(input_Move, unit) as CommandResult_Move;
                                    property_Position.Set(result_Move.postion);
                                }
                                break;
                        }
                    }
            }

            //应用模拟到现在的结果
            Vector3 aimPos = property_Position.Get();
            //Vector3 moveDelta = aimPos - unit.Position;
            unit.GetComponent<CharacterCtrComponent>().MoveTo(aimPos);

        }

    }
}
