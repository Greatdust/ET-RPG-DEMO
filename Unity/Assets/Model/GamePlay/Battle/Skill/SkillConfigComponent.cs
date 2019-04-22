using ETModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ObjectSystem]
public class SkillConfigComponentAwakeSystem : AwakeSystem<SkillConfigComponent>
{
    public override void Awake(SkillConfigComponent self)
    {
        self.Awake();
    }
}

public class SkillConfigComponent : ETModel.Component
{
    public SkillCollection skillCollection;

    public static SkillConfigComponent instance;

    Dictionary<string, ActiveSkillData> activeDatas;
    public const string abName = "skillconfig.unity3d";

    public void Awake()
    {
        instance = this;
        Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(abName);
        skillCollection = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(abName, "SkillCollection") as SkillCollection;
#if UNITY_EDITOR
        TestDeserialize();
#else
        activeDatas = skillCollection.activeSkillDataDic;
#endif
    }

    void TestDeserialize()
    {
        activeDatas = MessagePack.MessagePackSerializer.Deserialize<Dictionary<string, ActiveSkillData>>(File.ReadAllBytes(Application.dataPath+"../../../Config/ActiveSkillData.bytes"),
           MessagePack.Resolvers.ContractlessStandardResolver.Instance);
    }

    public ActiveSkillData GetActiveSkill(string skillId)
    {
        ActiveSkillData data;
        if (!activeDatas.TryGetValue(skillId, out data))
        {
            return null;
        }
        data.skillId = skillId;
        return data;
    }

    public PassiveSkillData GetPassiveSkill(string skillId)
    {
        PassiveSkillData data;
        if (!skillCollection.passiveSkillDataDic.TryGetValue(skillId, out data))
        {
            return null;
        }
        data.skillId = skillId;
        return data;
    }
}

