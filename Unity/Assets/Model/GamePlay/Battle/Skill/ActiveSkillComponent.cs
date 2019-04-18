using ETModel;
using MongoDB.Bson.Serialization.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
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

    public CancellationTokenSource cancelToken;//用以执行技能中断的

    public ETTaskCompletionSource<bool> tcs;

    public bool usingSkill;

    public void Awake()
    {
        skillList = new Dictionary<string, BaseSkill_AppendedData>();
    }

    #region 战斗流程
    public async ETVoid Execute(string skillId)
    {
        try
        {
            if (usingSkill) return;
            if (!skillList.ContainsKey(skillId)) return;
            if (!SkillHelper.CheckIfSkillCanUse(skillId, GetParent<Unit>())) return;
            ActiveSkillData activeSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(skillId);
            SkillHelper.ExcuteSkillParams excuteSkillParams = new SkillHelper.ExcuteSkillParams();
            excuteSkillParams.skillId = skillId;
            excuteSkillParams.source = GetParent<Unit>();
            excuteSkillParams.skillLevel = 1;
            bool canUse = await SkillHelper.CheckInput(excuteSkillParams);
            if (GetParent<Unit>() == UnitComponent.Instance.MyUnit)
            {
                //联网模式玩家主动使用技能需要等待服务器确认消息,以决定技能是否真的可以使用

                tcs = new ETTaskCompletionSource<bool>();
                canUse = await tcs.Task;
                tcs = null;
                if (!canUse) return;
            }
            // 联网模式非玩家单位使用技能直接跳过检测,因为是收到使用技能的确定消息了才开始执行技能.
            //TODO: 暂时先直接取消之前的行动
            cancelToken?.Cancel();
            Game.EventSystem.Run(EventIdType.CancelPreAction, GetParent<Unit>());
            CharacterStateComponent characterStateComponent = GetParent<Unit>().GetComponent<CharacterStateComponent>();
            characterStateComponent.Set(SpecialStateType.NotInControl, true);
            cancelToken = new CancellationTokenSource();
            excuteSkillParams.cancelToken = cancelToken;
            usingSkill = true;
            await SkillHelper.ExcuteActiveSkill(excuteSkillParams);
            cancelToken = null;
            characterStateComponent.Set(SpecialStateType.NotInControl, false);
            usingSkill = false;
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }

    }

    //中断可能正在执行的技能
    public void Interrupt()
    {
        CharacterStateComponent characterStateComponent = GetParent<Unit>().GetComponent<CharacterStateComponent>();
        if (characterStateComponent.Get(SpecialStateType.UnStoppable)) return;// 霸体状态,打断失败
        cancelToken?.Cancel();
        cancelToken = null;
        usingSkill = false;
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

