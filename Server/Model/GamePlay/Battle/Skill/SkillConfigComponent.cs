using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ObjectSystem]
public class SkillConfigComponentAwakeSystem : AwakeSystem<SkillConfigComponent>
{
    public override void Awake(SkillConfigComponent self)
    {
        self.Awake();
    }
}

public class SkillConfigComponent : Component
{
    public Dictionary<string, ActiveSkillData> activeSkillDataDic = new Dictionary<string, ActiveSkillData>();
    public Dictionary<string, PassiveSkillData> passiveSkillDataDic = new Dictionary<string, PassiveSkillData>();

    public static SkillConfigComponent instance;


    public const string abName = "skillconfig.unity3d";

    public void Awake()
    {
        instance = this;

    }

    public ActiveSkillData GetActiveSkill(string skillId)
    {
        ActiveSkillData data;
        if (!activeSkillDataDic.TryGetValue(skillId, out data))
        {
            return null;
        }
        data.skillId = skillId;
        return data;
    }

    public PassiveSkillData GetPassiveSkill(string skillId)
    {
        PassiveSkillData data;
        if (!passiveSkillDataDic.TryGetValue(skillId, out data))
        {
            return null;
        }
        data.skillId = skillId;
        return data;
    }
}

