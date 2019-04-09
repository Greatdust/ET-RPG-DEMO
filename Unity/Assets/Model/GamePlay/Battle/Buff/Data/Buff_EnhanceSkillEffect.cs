using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class Buff_EnhanceSkillEffect : BaseBuffData
{
    public string skillId;
    public SkillEffectComponent.EffectData effectData = new SkillEffectComponent.EffectData();

    public override string GetBuffIdType()
    {
        return BuffIdType.EnhanceSkillEffect;
    }
}
