using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillCollection : SerializedScriptableObject
{
    public Dictionary<string, ActiveSkillData> activeSkillDataDic = new Dictionary<string, ActiveSkillData>();
    public Dictionary<string,PassiveSkillData> passiveSkillDataDic =new Dictionary<string, PassiveSkillData>();
}
