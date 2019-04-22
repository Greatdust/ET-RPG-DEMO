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


        private Input_Move inputInfo_Move = new Input_Move();
        private Input_UseSkill_Pos Input_UseSkill_Pos = new Input_UseSkill_Pos();
        private Input_UseSkill_Dir Input_UseSkill_Dir = new Input_UseSkill_Dir();
        private Input_UseSkill_Tar Input_UseSkill_Tar = new Input_UseSkill_Tar();

        public int simulateFrame;//预测帧


        public int preActualFrame; //上一次收到服务器发来的确定帧

        private const int maxDiffFrame = 60;//当前帧和上一次服务器发来的帧之间差距超过多少,就不模拟了


        public UnitStateComponent unitState;

        public ETCancellationTokenSource moveCancelTokenSource;

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
            //TODO; 和上一次服务器确认帧相差多少之后,提示网络有问题
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
                           
                            //再预测这一帧的结果
                            unit.GetComponent<CharacterMoveComponent>().MoveAsync(result_Move.Path).Coroutine();

                            //单机模式不发送网络消息
                            if (!Game.Scene.GetComponent<GlobalConfigComponent>().networkPlayMode) break;
                            Log.Debug("frame : " + simulateFrame + "  本地预测的路径: " + result_Move.Path.ListToString<Vector3>());
                            inputInfo_Move.Frame = simulateFrame;
                            inputInfo_Move.AimPos = input_Move.clickPos.ToV3Info();
                            ETModel.Game.Scene.GetComponent<SessionComponent>().Session.Send(inputInfo_Move);

                            break;
                        case CommandInput_UseSkill input_UseSkill:

                            CommandResult_UseSkill result_UseSkill = simulaterComponent.commandSimulaters[input_UseSkill.GetType()].Simulate(input_UseSkill, unit) as CommandResult_UseSkill;
                            v.commandResult = result_UseSkill;

                            if (!Game.Scene.GetComponent<GlobalConfigComponent>().networkPlayMode)
                            {
                                //单机的话本地直接做决定了
                                unit.GetComponent<ActiveSkillComponent>().tcs?.SetResult(result_UseSkill.success);
                                break;
                            }

                            switch (input_UseSkill.bufferValue)
                            {
                                case BufferValue_Pos value_Pos:
                                    Input_UseSkill_Pos.Frame = simulateFrame;
                                    Input_UseSkill_Pos.SkillId = input_UseSkill.skillId;
                                    Input_UseSkill_Pos.PipelineSignal = input_UseSkill.pipelineSignal;
             
                                    Input_UseSkill_Pos.AimPos = value_Pos.aimPos.ToV3Info();
                                    ETModel.Game.Scene.GetComponent<SessionComponent>().Session.Send(Input_UseSkill_Pos);
                                    break;
                                case BufferValue_Dir value_Dir:
                                    Input_UseSkill_Dir.Frame = simulateFrame;
                                    Input_UseSkill_Dir.SkillId = input_UseSkill.skillId;
                                    Input_UseSkill_Dir.PipelineSignal = input_UseSkill.pipelineSignal;

                                    Input_UseSkill_Dir.AimDir = value_Dir.dir.ToV3Info();
                                    ETModel.Game.Scene.GetComponent<SessionComponent>().Session.Send(Input_UseSkill_Dir);
                                    break;
                                case BufferValue_TargetUnits value_TargetUnits:
                                    Input_UseSkill_Tar.Frame = simulateFrame;
                                    Input_UseSkill_Tar.SkillId = input_UseSkill.skillId;
                                    Input_UseSkill_Tar.PipelineSignal = input_UseSkill.pipelineSignal;

                                    Input_UseSkill_Tar.UnitId = value_TargetUnits.targets[0].Id;
                                    ETModel.Game.Scene.GetComponent<SessionComponent>().Session.Send(Input_UseSkill_Tar);
                                    break;
                                default:
                                    break;
                            }

                            break;
                    }
                }
            }

        }

        public void CollectCommandInput(ICommandInput input)
        {
            CharacterStateComponent property_CharacterState = GetParent<Unit>().GetComponent<CharacterStateComponent>();
            if (property_CharacterState.Get(SpecialStateType.CantDoAction) || property_CharacterState.Get(SpecialStateType.NotInControl))
            {
                return;
            }
            collectedNewInput = true;
            Command command = CommandGCHelper.GetCommand();
            command.commandInput = input;
            currCommands[input.GetType()] = command;

        }

        public void GetCommandResult(UnitStateDelta unitStateDelta)
        {

            //延迟太高了,迟迟收不到服务器发过来的确认消息
            if (simulateFrame - unitStateDelta.frame > maxDiffFrame)
            {
                Log.Debug("拉扯回来");
                simulateFrame = unitStateDelta.frame;
                preActualFrame = simulateFrame;
                //拉扯回来

                switch (unitStateDelta.commandResult)
                {
                    case CommandResult_Move result:
                        //设置当前位置
                        unit.Position = result.Path[0];

                        //再预测这一帧的结果
                        unit.GetComponent<CharacterMoveComponent>().MoveAsync(result.Path).Coroutine();
                        break;
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
            {
                switch (unitStateDelta.commandResult)
                {
                    case CommandResult_Move actualMove:
                        foreach (var v in cacheCommands[unitStateDelta.frame])
                        {
                            if (v.commandResult is CommandResult_Move simulateMove)
                            {
                                Log.Debug("frame : " + unitStateDelta.frame + "  收到服务器发过来的路径: " + actualMove.Path.ListToString<Vector3>());
                                if (Vector3.Distance(simulateMove.Path[0], actualMove.Path[0]) < 0.05f
                                    && Vector3.Distance(simulateMove.Path[simulateMove.Path.Count - 1], actualMove.Path[actualMove.Path.Count - 1]) < 0.05f)
                                {

                                    break;
                                }
                                else
                                {
                                    needRecal = true;
                                    unit.Position = actualMove.Path[0];
                                    break;
                                }
                            }
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

            for (int i = simulateFrame; i >= startFrame; i--)
            {
                if (cacheCommands.ContainsKey(i))
                    foreach (var v in cacheCommands[i])
                    {

                        switch (v.commandInput)
                        {
                            //移动的话,直接取最后一次的目标作为目标就好了.
                            case CommandInput_Move input_Move:

                                CommandResult_Move result_Move = simulaterComponent.commandSimulaters[input_Move.GetType()].Simulate(input_Move, unit) as CommandResult_Move;
                                v.commandResult = result_Move;
                                //直接拉扯
                                unit.GetComponent<CharacterMoveComponent>().MoveAsync(result_Move.Path).Coroutine();
                                break;
                        }
                        return;
                    }
            }
     


        }

    }
}
