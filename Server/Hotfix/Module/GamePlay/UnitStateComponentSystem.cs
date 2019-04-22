using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

namespace ETHotfix
{

    public static class UnitStateComponentSystem
    {
        public static void FixedUpdate(this UnitStateComponent unitStateComponent)
        {

            if (!unitStateComponent.haveInited) return;
            if (unitStateComponent.unitStatesDic.Count == 0) return;
            UnitStateMgrComponent mgr = Game.Scene.GetComponent<UnitStateMgrComponent>();
            Log.Info(string.Format("frame {0} : 玩家位置信息 {1}", mgr.currFrame, unitStateComponent.unit.Position));
            //每间隔3帧发一次数据
            if (mgr.currFrame - unitStateComponent.preSendMsgFrame < UnitStateMgrComponent.sendMsgDelta)
            {
                return;
            }
            unitStateComponent.preSendMsgFrame = mgr.currFrame;

            if (!unitStateComponent.collectInput)
            {
                return;
            }

            //TODO : 这里有大量GC,需要处理
            //if (unitStateComponent.currGetInputFrame - unitStateComponent.preClearInputFrame >= UnitStateComponent.maxFrameCount_SaveStateDelta)
            //{
            //    for (int i = unitStateComponent.preClearInputFrame; i < unitStateComponent.currGetInputFrame - UnitStateComponent.maxFrameCount_SaveStateDelta; i++)
            //    {
            //        if (unitStateComponent.unitStatesDic.ContainsKey(i))
            //        {
            //            unitStateComponent.unitStatesDic.Remove(i);
            //        }
            //    }
            //}

            //每次发送都发最新的的结果
            //var state = unitStateComponent.unitStatesDic[unitStateComponent.currGetInputFrame];

            //foreach (var v in state.commandResults)
            //{
            //    CommandResultInfo_Move commandResultInfo_Move = new CommandResultInfo_Move();
            //    commandResultInfo_Move.Frame = state.frame;

            //    switch (v.Value)
            //    {
            //        case CommandResult_Move result_Move:
                        
            //            continue;
            //    }
            //}
            unitStateComponent.collectInput = false;


        }


        //玩家刚开始的时候初始化所有的属性
        public static void Init(this UnitStateComponent unitStateComponent, Dictionary<Type, IProperty> unitProperty)
        {
            unitStateComponent.unitProperty = unitProperty;
            unitStateComponent.haveInited = true;
            unitStateComponent.preSendMsgFrame = -3;
        }

        public static void GetInput(this UnitStateComponent unitStateComponent, int frame, ICommandInput commandInput)
        {
            try
            {
                //unitStateComponent.collectInput = true;
                //if (unitStateComponent.currGetInputFrame < frame)
                //    unitStateComponent.currGetInputFrame = frame;
                //UnitStateDelta unitStateDelta = new UnitStateDelta();
                //unitStateDelta.frame = frame;

                var result = Game.Scene.GetComponent<CommandSimulaterComponent>().commandSimulaters[commandInput.GetType()].Simulate(commandInput, unitStateComponent.unit);
                switch (result)
                {
                    case CommandResult_Move result_Move:
                        unitStateComponent.unit.GetComponent<CharacterMoveComponent>().MoveAsync(result_Move.Path).Coroutine();

                        unitStateComponent.inputResult_Move.Frame = frame;
                        unitStateComponent.inputResult_Move.Id = unitStateComponent.unit.Id;
                        unitStateComponent.inputResult_Move.PathList = new Google.Protobuf.Collections.RepeatedField<Vector3Info>();
                        for (int i = 0; i < result_Move.Path.Count; i++)
                        {
                            unitStateComponent.inputResult_Move.PathList.Add(new Vector3Info()
                            {
                                X = result_Move.Path[i].x,
                                Y = result_Move.Path[i].y,
                                Z = result_Move.Path[i].z
                            });
                        }
                        MessageHelper.Broadcast(unitStateComponent.inputResult_Move);
                        break;
                    case CommandResult_UseSkill result_UseSkill:

                        bool checkResult = unitStateComponent.unit.GetComponent<ActiveSkillComponent>().CheckCanUse(result_UseSkill.skillId);
                        if (checkResult)
                            unitStateComponent.unit.GetComponent<ActiveSkillComponent>().Execute(result_UseSkill.skillId).Coroutine();
                        switch ((commandInput as CommandInput_UseSkill).bufferValue)
                        {
                            case BufferValue_Dir bufferValue_Dir:
                                unitStateComponent.useSkill_Dir.SkillId = result_UseSkill.skillId;
                                unitStateComponent.useSkill_Dir.Success = checkResult;
                                unitStateComponent.useSkill_Dir.Id = unitStateComponent.unit.Id;
                                unitStateComponent.useSkill_Dir.PipelineSignal = (commandInput as CommandInput_UseSkill).pipelineSignal;
                                unitStateComponent.useSkill_Dir.Dir = bufferValue_Dir.dir.ToV3Info();
                                MessageHelper.Broadcast(unitStateComponent.useSkill_Dir);
                                break;

                        }
                        break;
                }
                //unitStateDelta.commandResults.Add(result.GetType(), result);
                //unitStateComponent.unitStatesDic[unitStateComponent.currGetInputFrame] = unitStateDelta;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());

            }
        }

    }
}
