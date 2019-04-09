using ETModel;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ObjectSystem]
public class SkillEffectComponentAwakeSystem : AwakeSystem<SkillEffectComponent>
{
    public override void Awake(SkillEffectComponent self)
    {
        self.Awake();
    }
}


public class SkillEffectComponent:ETModel.Component
{
    [System.Serializable]
    public class EffectData
    {
        public float coefficientAddPct;//效果增加百分比,如果该效果受系数影响,那就是系数临时提高,如果不受,那就是改变的值提高
        public bool critical;//如果是伤害/治疗,必定暴击
    }
    public Dictionary<string, List<EffectData>> effectDataDic;

    public void Awake()
    {
        effectDataDic = new Dictionary<string, List<EffectData>>();
    }

    public EffectData GetEffectData(string skillId)
    {
        EffectData effectData = new EffectData();
        effectData.coefficientAddPct = 0;
        effectData.critical = false;
        List<EffectData> list = null;
        if (effectDataDic.TryGetValue(skillId, out list))
        {
            foreach (var v in list)
            {
                effectData.coefficientAddPct += v.coefficientAddPct;
                if (v.critical) effectData.critical = true;
            }
        }
        return effectData;
    }

    public void AddEffectData(string skillId, EffectData effectData)
    {
        if (!effectDataDic.ContainsKey(skillId))
        {
            effectDataDic[skillId] = new List<EffectData>();
        }
        effectDataDic[skillId].Add(effectData);
    }

    public void RemoveEffectData(string skillId, EffectData effectData)
    {
        List<EffectData> list = null;
        if (effectDataDic.TryGetValue(skillId, out list))
        {
            if (list.Contains(effectData))
            {
                list.Remove(effectData);
            }
        }
    }
}
