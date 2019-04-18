using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PF;
using UnityEngine;

namespace ETModel
{
    [CommandInput(typeof(CommandInput_UseSkill))]
    public class CommandInputSystem_UseSkill: ICommandSimulater
    {

        public ICommandResult Simulate(ICommandInput commandInput, Unit unit)
        {
            CommandInput_UseSkill input_UseSkill = commandInput as CommandInput_UseSkill;
            if (!SkillHelper.tempData.ContainsKey((unit, input_UseSkill.pipelineSignal)))
            {
                SkillHelper.tempData[(unit, input_UseSkill.pipelineSignal)] = new Dictionary<Type, IBufferValue>();
            }
            SkillHelper.tempData[(unit, input_UseSkill.pipelineSignal)][input_UseSkill.bufferValue.GetType()] = input_UseSkill.bufferValue;

            //TODO:做一些检查,判定技能释放是否可以成功. 这里DEMO的话直接成功算了.

            CommandResult_UseSkill result_UseSkill = CommandGCHelper.GetCommandResult<CommandResult_UseSkill>();
            result_UseSkill.skillId = input_UseSkill.skillId;
            result_UseSkill.success = true;
            // result_Move.dir = input_Move.moveDir;// 暂时就以输入的方向作为角色的方向
            return result_UseSkill;

        }
    }
}
