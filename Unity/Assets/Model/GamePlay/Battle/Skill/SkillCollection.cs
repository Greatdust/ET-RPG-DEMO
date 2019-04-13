using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

[CreateAssetMenu(menuName = "游戏设置/技能配置", fileName = "SkillCollection")]
public class SkillCollection : SerializedScriptableObject
{
    public Dictionary<string, ActiveSkillData> activeSkillDataDic = new Dictionary<string, ActiveSkillData>();
    public Dictionary<string,PassiveSkillData> passiveSkillDataDic =new Dictionary<string, PassiveSkillData>();

#if UNITY_EDITOR
    [Button("保存所有技能信息至文件")]
    public void SaveToFile()
    {
        using (FileStream file = File.Create(Application.dataPath + "../../../Config/ActiveSkillData.bytes"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            //序列化
            bf.Serialize(file, activeSkillDataDic);
            file.Close();
        }
        using (FileStream file = File.Create(Application.dataPath + "../../../Config/PassiveSkillData.bytes"))
        {
            BinaryFormatter bf = new BinaryFormatter();
            //序列化
            bf.Serialize(file, passiveSkillDataDic);
            file.Close();
        }
    }
#endif
}
