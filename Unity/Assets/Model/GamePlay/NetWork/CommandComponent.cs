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

        public int simulateFrame;//预测帧


        public int preActualFrame; //上一次收到服务器发来的确定帧

        private const int maxDiffFrame = 60;//当前帧和上一次服务器发来的帧之间差距超过多少,就不模拟了


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
            simulateFrame++;
            //先上传这一帧的操作,同时缓存操作,然后客户端自己模拟操作的结果
            if (collectedNewInput)
            {
                collectedNewInput = false;
                cacheCommands[simulateFrame] = new List<Command>();
                cacheCommands[simulateFrame].AddRange(currCommands.Values);
                currCommands.Clear();
                foreach (var v in cacheCommands[simulateFrame])
                {

                    switch (v.commandInput)
                    {
                        case CommandInput_Move input_Move:

                            CommandResult_Move result_Move = simulaterComponent.commandSimulaters[input_Move.GetType()].Simulate(input_Move, unit) as CommandResult_Move;
                            v.commandResult = result_Move;

                            //每秒最多发送60次
                            inputInfo_Move.Frame = simulateFrame;
                            inputInfo_Move.MoveDir = new Vector3Info() { X = input_Move.moveDir.x, Y = input_Move.moveDir.y, Z = input_Move.moveDir.z };
                            ETModel.Game.Scene.GetComponent<SessionComponent>().Session.Send(inputInfo_Move);

                            Log.Debug(string.Format("预测位置{3}:   {0},{1},{2}", result_Move.postion.x, result_Move.postion.y, result_Move.postion.z, simulateFrame));
                            //再预测这一帧的结果
                            //Log.Debug("frame : " + simulateFrame + " 预测位置:" + result_Move.postion);
                            //unit.GetComponent<CharacterCtrComponent>().MoveTo(result_Move.postion);
                            break;
                    }
                }
            }



        }

        public void CollectCommandInput(ICommandInput input)
        {
            collectedNewInput = true;
            Command command = CommandGCHelper.GetCommand();
            command.commandInput = input;
            currCommands[input.GetType()] = command;

        }

        public void GetCommandResult(UnitStateDelta unitStateDelta)
        {
            foreach (var v in unitStateDelta.commandResults.Values)
            {
                switch (v)
                {
                    case CommandResult_Move result:
                        Log.Debug(string.Format("服务器发送过来的位置{3}:   {0},{1},{2}", result.postion.x, result.postion.y, result.postion.z, unitStateDelta.frame));
                        unit.GetComponent<CharacterCtrComponent>().MoveTo(result.postion);
                        continue;
                }
            }
            return;
            //延迟太高了,迟迟收不到服务器发过来的确认消息
            if (simulateFrame - preActualFrame > maxDiffFrame)
            {
                simulateFrame = preActualFrame;
                //拉扯回来
                foreach (var v in unitStateDelta.commandResults.Values)
                {
                    switch (v)
                    {
                        case CommandResult_Move result:
                            unit.Position = result.postion;
                            continue;
                    }
                }
                return;
            }
            if (preActualFrame != unitStateDelta.frame)
            {
                //移除上一次的确定帧和接收到的这一帧之间的缓存指令
                for (int i = preActualFrame; i < unitStateDelta.frame; i++)
                {
                    if (cacheCommands.ContainsKey(i))
                    {
                        foreach (var v in cacheCommands[i])
                        {
                            CommandGCHelper.Recycle(v);
                        }
                        cacheCommands.Remove(i);
                    }
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
                            Log.Debug("frame  " + unitStateDelta.frame + " 服务器发来的目标位置  " + actualMove.postion.ToString());
                            Log.Debug("本地计算的当前位置" + simulateMove.postion.ToString());
                            if (Vector3.Distance(simulateMove.postion, actualMove.postion) < 0.05f)
                            {
                                continue;
                            }
                            else
                            {
                              
                                simulateMove.postion = actualMove.postion;
                                needRecal = true;
                                unit.Position = actualMove.postion;
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

            for (int i = startFrame; i <= simulateFrame; i++)
            {
                if (cacheCommands.ContainsKey(i))
                    foreach (var v in cacheCommands[i])
                    {

                        switch (v.commandInput)
                        {
                            case CommandInput_Move input_Move:
                                CommandResult_Move result_Move = simulaterComponent.commandSimulaters[input_Move.GetType()].Simulate(input_Move, unit) as CommandResult_Move;
                                v.commandResult = result_Move;
                                unit.Position = result_Move.postion;
                                break;
                        }
                    }
            }


        }

    }
}
