using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[LabelText("提升技能效果")]
[LabelWidth(150)]
[Serializable]
public class Buff_EnhanceSkillEffect : BaseBuffData
{
    public string skillId;
    public SkillEffectComponent.EffectData effectData;

    public float growthCoff;// 系数随技能等级的成长值 

    public override string GetBuffIdType()
    {
        return BuffIdType.EnhanceSkillEffect;
    }
}
