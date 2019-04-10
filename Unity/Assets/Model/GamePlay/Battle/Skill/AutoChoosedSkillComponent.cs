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
    public ChooseSkill_Damage damageSkill = new ChooseSkill_Damage();
    public ChooseSkill_Buff buffSkill =new ChooseSkill_Buff();
    public ChooseSkill_Restore restoreSkill =new ChooseSkill_Restore();


    public Dictionary<int, IAutoChoosedSkillCondition> conditions = new Dictionary<int, IAutoChoosedSkillCondition>();


    public void Awake()
    {
        //TODO:后续要加载单位自己的配置,这里统一用默认的
        conditions.Add(buffSkill.Priority, buffSkill);
        conditions.Add(damageSkill.Priority, damageSkill);
        conditions.Add(restoreSkill.Priority, restoreSkill);

    }


    public ActiveSkillData GetSkillData(Unit unit)
    {
        ActiveSkillComponent skillCom = unit.GetComponent<ActiveSkillComponent>();
        ActiveSkillData skillData = null;
        for (int i = 0; i < conditions.Count; i++)
        {
            skillData = conditions[i].GetActiveSkillData(unit);
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

