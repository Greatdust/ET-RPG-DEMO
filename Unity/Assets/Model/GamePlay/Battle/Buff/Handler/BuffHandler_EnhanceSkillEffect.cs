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
    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_EnhanceSkillEffect buff = data as Buff_EnhanceSkillEffect;
        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            SkillEffectComponent skillEffectComponent = target.GetComponent<SkillEffectComponent>();
            skillEffectComponent.AddEffectData(buff.skillId, buff.effectData);
        }
    }
}



