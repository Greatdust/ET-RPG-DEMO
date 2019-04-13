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
    public SkillEffectComponent.EffectData effectData;

    public float growthCoff;// 系数随技能等级的成长值 

    public override string GetBuffIdType()
    {
        return BuffIdType.EnhanceSkillEffect;
    }
}
