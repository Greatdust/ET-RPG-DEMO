using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;



[ObjectSystem]
public class AutoChoosedSkillComponentAwakeSystem : AwakeSystem<AutoChoosedSkillComponent>
{
    public override void Awake(AutoChoosedSkillComponent self)
    {
        self.Awake();
    }
}

public class AutoChoosedSkillComponent : ETModel.Component
{

    public ChooseSkill_单体Buff类 _单体Buff =new ChooseSkill_单体Buff类();
    public ChooseSkill_单体伤害类 _单体伤害 =new ChooseSkill_单体伤害类();
    public ChooseSkill_单体治疗类 _单体治疗 =new ChooseSkill_单体治疗类();
    public ChooseSkill_群体Buff类 _群体Buff =new ChooseSkill_群体Buff类();
    public ChooseSkill_群体伤害类 _群体伤害 =new ChooseSkill_群体伤害类();
    public ChooseSkill_群体治疗类 _群体治疗 = new ChooseSkill_群体治疗类();

    public Dictionary<int, IAutoChoosedSkillCondition> conditions = new Dictionary<int, IAutoChoosedSkillCondition>();


    public void Awake()
    {
        //TODO:后续要加载玩家自己的配置,这里统一用默认的
        conditions.Add(_单体Buff.Priority, _单体Buff);
        conditions.Add(_单体伤害.Priority, _单体伤害);
        conditions.Add(_群体Buff.Priority, _群体Buff);
        conditions.Add(_群体伤害.Priority, _群体伤害);
        conditions.Add(_单体治疗.Priority, _单体治疗);
        conditions.Add(_群体治疗.Priority, _群体治疗);
    }


    public ActiveSkillData GetSkillData(UnitActionData data)
    {
        ActiveSkillComponent skillCom = data.mUnit.GetComponent<ActiveSkillComponent>();
        ActiveSkillData skillData = null;
        for (int i = 0; i < conditions.Count; i++)
        {
            skillData = conditions[i].GetActiveSkillData(data);
            if (skillData != null)
            {
                Log.Debug(skillData.skillName);
                return skillData;
            }
        }

        return skillCom.Skill_NormalAttack;
    }

    public override void Dispose()
    {
        base.Dispose();
        conditions.Clear();
    }
}

