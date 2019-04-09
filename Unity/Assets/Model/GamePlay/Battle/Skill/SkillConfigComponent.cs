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
    public SkillCollection skillCollection;

    public static SkillConfigComponent instance;


    public const string abName = "skillconfig.unity3d";

    public void Awake()
    {
        instance = this;
        Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(abName);
        skillCollection = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(abName, "SkillCollection") as SkillCollection;
        Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(abName);

    }

    public ActiveSkillData GetDeepCopy_ActiveSkill(string skillId)
    {
        ActiveSkillData data;
        if (!skillCollection.activeSkillDataDic.TryGetValue(skillId, out data))
        {
            return null;
        }
        ActiveSkillData skillData = DeepCopyHelper.DeepCopyByBin<ActiveSkillData>(data);
        skillData.skillExcuteSpeed = 1;
        return skillData;
    }

    public PassiveSkillData GetDeepCopy_PassiveSkill(string skillId)
    {
        PassiveSkillData data;
        if (!skillCollection.passiveSkillDataDic.TryGetValue(skillId, out data))
        {
            return null;
        }
        PassiveSkillData skillData = DeepCopyHelper.DeepCopyByBin<PassiveSkillData>(data);
        skillData.skillExcuteSpeed = 1;
        return skillData;
    }
}

