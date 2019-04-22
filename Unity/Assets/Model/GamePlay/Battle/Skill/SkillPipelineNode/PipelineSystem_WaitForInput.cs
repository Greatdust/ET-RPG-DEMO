using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class PipelineSystem_WaitForInput
{
    public static async ETTask<bool> Execute(this Pipeline_WaitForInput pipeline_WaitForInput, SkillHelper.ExecuteSkillParams skillParams)
    {
        await ETTask.CompletedTask;
#if !SERVER
        switch (pipeline_WaitForInput.inputType)
        {

            //等待用户输入,可能有正确输入/取消/输入超时三种情况

            case InputType.Tar:
                CommandInput_UseSkill commandInput_UseSkill = new CommandInput_UseSkill();

                if (skillParams.source != UnitComponent.Instance.MyUnit)
                {
                    //非玩家角色使用技能直接跳过等待输入步骤.
                    //非玩家角色使用技能的输入来自于其他地方(服务器下发)
                    return true;
                }

                if (SkillHelper.tempData.ContainsKey((skillParams.source, pipeline_WaitForInput.pipelineSignal)))
                {
                    //可能还存在着之前使用技能时找到的数据. 所以这里清理掉它
                    SkillHelper.tempData.Remove((skillParams.source, pipeline_WaitForInput.pipelineSignal));
                }
                BufferValue_TargetUnits bufferValue_TargetUnits = new BufferValue_TargetUnits();
                bufferValue_TargetUnits.targets = new Unit[1];
                if (UnitComponent.Instance.MyUnit.GetComponent<InputComponent>().GetInputTarget(out bufferValue_TargetUnits.targets[0], pipeline_WaitForInput.findFriend, skillParams.source.UnitData.groupIndex))
                {
                    UnityEngine.Vector3 _forward1 = bufferValue_TargetUnits.targets[0].Position - skillParams.source.Position;
                    skillParams.source.Rotation = UnityEngine.Quaternion.LookRotation(new UnityEngine.Vector3(_forward1.x, 0, _forward1.z), UnityEngine.Vector3.up);


                    commandInput_UseSkill.skillId = skillParams.skillId;
                    commandInput_UseSkill.pipelineSignal = pipeline_WaitForInput.pipelineSignal;
                    commandInput_UseSkill.bufferValue = bufferValue_TargetUnits;
                    UnitComponent.Instance.MyUnit.GetComponent<CommandComponent>().CollectCommandInput(commandInput_UseSkill);
                }
                else
                {
                    Log.Debug("找不到目标! 技能终止!");
                    return false;
                }

                break;
            case InputType.Dir:
                commandInput_UseSkill = new CommandInput_UseSkill();
                if (skillParams.source != UnitComponent.Instance.MyUnit)
                {
                    //非玩家角色使用技能直接跳过等待输入步骤.
                    //非玩家角色使用技能的输入来自于其他地方(服务器下发)
                    return true;
                }

                if (SkillHelper.tempData.ContainsKey((skillParams.source, pipeline_WaitForInput.pipelineSignal)))
                {
                    //可能还存在着之前使用技能时找到的数据. 所以这里清理掉它
                    SkillHelper.tempData.Remove((skillParams.source, pipeline_WaitForInput.pipelineSignal));
                }
                //直接智能施法模式
                BufferValue_Dir bufferValue_Dir = new BufferValue_Dir();
                bufferValue_Dir.dir = UnitComponent.Instance.MyUnit.GetComponent<InputComponent>().GetInputDir();

                //直接改变使用技能时角色的转向
                skillParams.source.Rotation = UnityEngine.Quaternion.LookRotation(bufferValue_Dir.dir, UnityEngine.Vector3.up);

                commandInput_UseSkill.skillId = skillParams.skillId;
                commandInput_UseSkill.pipelineSignal = pipeline_WaitForInput.pipelineSignal;
                commandInput_UseSkill.bufferValue = bufferValue_Dir;
                UnitComponent.Instance.MyUnit.GetComponent<CommandComponent>().CollectCommandInput(commandInput_UseSkill);

                break;
            case InputType.Pos:
                commandInput_UseSkill = new CommandInput_UseSkill();
                if (skillParams.source != UnitComponent.Instance.MyUnit)
                {
                    //非玩家角色使用技能直接跳过等待输入步骤.
                    //非玩家角色使用技能的输入来自于其他地方(服务器下发)
                    return true;
                }

                if (SkillHelper.tempData.ContainsKey((skillParams.source, pipeline_WaitForInput.pipelineSignal)))
                {
                    //可能还存在着之前使用技能时找到的数据. 所以这里清理掉它
                    SkillHelper.tempData.Remove((skillParams.source, pipeline_WaitForInput.pipelineSignal));
                }
                BufferValue_Pos bufferValue_Pos = new BufferValue_Pos();
                if (!UnitComponent.Instance.MyUnit.GetComponent<InputComponent>().GetInputPos(out bufferValue_Pos.aimPos))
                {
                    return false;
                }

                //直接改变使用技能时角色的转向
                UnityEngine.Vector3 forward = bufferValue_Pos.aimPos - skillParams.source.Position;
                skillParams.source.Rotation = UnityEngine.Quaternion.LookRotation(new UnityEngine.Vector3(forward.x, 0, forward.z), UnityEngine.Vector3.up);

                commandInput_UseSkill.skillId = skillParams.skillId;
                commandInput_UseSkill.pipelineSignal = pipeline_WaitForInput.pipelineSignal;
                commandInput_UseSkill.bufferValue = bufferValue_Pos;
                UnitComponent.Instance.MyUnit.GetComponent<CommandComponent>().CollectCommandInput(commandInput_UseSkill);
                break;
            case InputType.Charge:
                break;
            case InputType.Spell:

                break;
            case InputType.ContinualSpell:

                SpellForTime(pipeline_WaitForInput.value, skillParams).Coroutine();
                break;


    }
#else
        switch (pipeline_WaitForInput.inputType)
        {
            case InputType.Tar:
                break;
            case InputType.Dir:
                break;
            case InputType.Pos:
                break;
            case InputType.Charge:
                break;
            case InputType.Spell:
                break;
            case InputType.ContinualSpell:
                SpellForTime(pipeline_WaitForInput.value, skillParams).Coroutine();
                break;
        }
#endif
        return true;
    }
    async static ETVoid SpellForTime(float value, SkillHelper.ExecuteSkillParams skillParams)
    {
        CharacterStateComponent characterStateComponent = skillParams.source.GetComponent<CharacterStateComponent>();
        characterStateComponent.Set(SpecialStateType.NotInControl, true);
        skillParams.cancelToken.Token.Register(() =>
        {
            characterStateComponent.Set(SpecialStateType.NotInControl, false);
        });

        await TimerComponent.Instance.WaitAsync((long)(value * 1000), skillParams.cancelToken.Token);
        characterStateComponent.Set(SpecialStateType.NotInControl, false);
    }
}
