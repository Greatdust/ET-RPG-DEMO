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

    public bool usingSkill; // 判定是否尝试使用某个技能. 在此期间不允许使用其他技能.
    public string currUsingSkillId; //当前正在使用的技能


    public void Awake()
    {
        skillList = new Dictionary<string, BaseSkill_AppendedData>();
    }

    //中断可能正在执行的技能
    public void Interrupt(TypeOfInterruption type)
    {
        //TODO: 根据当前使用技能允许的可打断类型判定打断是否可以成功

        CharacterStateComponent characterStateComponent = GetParent<Unit>().GetComponent<CharacterStateComponent>();
        if (characterStateComponent.Get(SpecialStateType.UnStoppable)) return;// 霸体状态,打断失败
        cancelToken?.Cancel();
        cancelToken = null;
    }

}

