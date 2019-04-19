using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public static class PipelineSystem_Programmable
{
    public static void Execute(this Pipeline_Programmable pipeline_Programmable,SkillHelper.ExecuteSkillParams skillParams)
    {
        SkillData_Var skillData_Var = default;
        skillData_Var.pipelineSignal = pipeline_Programmable.pipelineSignal;
        skillData_Var.skillId = skillParams.skillId;
        skillData_Var.source = skillParams.source;
        skillParams.cancelToken.Token.Register(() => pipeline_Programmable.pmb.Break(skillData_Var));
        pipeline_Programmable.pmb.Excute(skillData_Var);
    }
}
