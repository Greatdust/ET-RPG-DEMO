using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[ObjectSystem]
public class SkillComponentAwakeSystem : AwakeSystem<ActiveSkillComponent>
{
    public override void Awake(ActiveSkillComponent self)
    {
        self.Awake();
    }
}

/// <summary>
/// 每个战斗单位身上都会有的一个主动技能组件，用以管理单位身上的主动技能
/// </summary>
public class ActiveSkillComponent : ETModel.Component
{
    public Dictionary<string, ActiveSkillData> activeSkillDic;//这里不会保存普通攻击技能


    public ActiveSkillData Skill_NormalAttack ;//记录一下普攻，单独抽出来

    private bool hasRequestedSkillTarget;//记录一下是否已经请求过技能目标了


    public void Awake()
    {
        activeSkillDic = new Dictionary<string, ActiveSkillData>();
    }
    #region 战斗流程
    public async ETVoid Excute(UnitActionData data)
    {
        hasRequestedSkillTarget = false;
        try
        {
            var currSkillData = await SkillHelper.GetActiveSkillData(data);
            Log.Debug(currSkillData.skillName);

            await SkillHelper.ExcuteActiveSkill(currSkillData);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

    }



    #endregion
    #region 技能添加,删除,获取

    public void AddSkill(ActiveSkillData skillData)
    {
        skillData.SourceUnit = GetParent<Unit>();
        foreach (var v in skillData.AllBuffInSkill.Values)
        {
            v.ParentSkillData = skillData;
        }

        if (skillData.isNormalAttack)
        {
            Skill_NormalAttack = skillData;
        }

        skillData.buffReturnValues = new Dictionary<string, List<IBuffReturnedValue>>();
        activeSkillDic[skillData.skillId] = skillData;
    }

    public void RemoveSkill(string skillId)
    {
        ActiveSkillData skillData = GetSkill(skillId);
        if (skillData == null || skillData == Skill_NormalAttack) return;
        activeSkillDic.Remove(skillId);
    }

    public ActiveSkillData GetSkill(string skillId)
    {
        ActiveSkillData data;
        if (!activeSkillDic.TryGetValue(skillId, out data))
        {
            return null;
        }
        return data;
    }
    #endregion
}

