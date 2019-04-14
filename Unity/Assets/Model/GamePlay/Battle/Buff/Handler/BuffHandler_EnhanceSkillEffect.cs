using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.EnhanceSkillEffect)]
public class BuffHandler_EnhanceSkillEffect : BaseBuffHandler,IBuffActionWithGetInputHandler
{
    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_EnhanceSkillEffect buff = (Buff_EnhanceSkillEffect)buffHandlerVar.data;

        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }

        foreach (var v in targetUnits.targets)
        {
            SkillEffectComponent skillEffectComponent = v.GetComponent<SkillEffectComponent>();
            skillEffectComponent.AddEffectData(buff.skillId, buff.effectData);
        }
    }
}



