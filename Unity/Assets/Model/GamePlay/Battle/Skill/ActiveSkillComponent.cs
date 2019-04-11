using ETModel;
using MongoDB.Bson.Serialization.Attributes;
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

    public Dictionary<string, BaseSkill_AppendedData> skillList;

    public string Skill_NormalAttack ;//记录一下普攻，单独抽出来

    //string 是对应的BuffSignal
    public Dictionary<string, Dictionary<Type,IBufferValue>> buffReturnValues = new Dictionary<string, Dictionary<Type, IBufferValue>>();//存储技能执行过程中产生的中间数据

    public Dictionary<string, Action> collisionEvents = new Dictionary<string, Action>();//存储技能执行过程中产生的碰撞事件,string是对应的BuffSignal

    public ETCancellationTokenSource cancelToken;//用以执行技能中断的

    public void Awake()
    {
        skillList = new Dictionary<string, BaseSkill_AppendedData>();
    }

    public void AddReturnValue(string buffSignal, IBufferValue buffReturnedValue)
    {
        if (!buffReturnValues.TryGetValue(buffSignal, out var dic))
        {
            dic = new Dictionary<Type, IBufferValue>();
            buffReturnValues[buffSignal] = dic;
        }
        dic[buffReturnedValue.GetType()] = buffReturnedValue;
    }

    #region 战斗流程
    public async ETVoid Excute(string skillId)
    {
        try
        {
            if (!skillList.ContainsKey(skillId)) return;
            ActiveSkillData activeSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(skillId);
            cancelToken = new ETCancellationTokenSource();
            await SkillHelper.ExcuteActiveSkill(activeSkillData, cancelToken);
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

    }



    #endregion
    #region 技能添加,删除,获取

    public void AddSkill(string skillId)
    {
        ActiveSkillData activeSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(skillId);
        if (activeSkillData.isNormalAttack)
        {
            Skill_NormalAttack = skillId;
        }
        if (!skillList.ContainsKey(skillId))
        {
            skillList.Add(skillId, new BaseSkill_AppendedData() { level = 1 });
        }
    }

    public void RemoveSkill(string skillId)
    {
        if (!skillList.ContainsKey(skillId)) return;
        if (skillId == Skill_NormalAttack) return;
        skillList.Remove(skillId);
    }

    public BaseSkill_AppendedData GetSkillAppendedData(string skillId)
    {
        if (skillList.TryGetValue(skillId, out var data))
            return data;
        else
            return null;
    }
    #endregion
}

