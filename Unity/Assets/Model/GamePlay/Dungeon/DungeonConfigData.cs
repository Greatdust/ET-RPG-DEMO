using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;

namespace ETModel
{
    //一个副本一个配置文件
    [CreateAssetMenu(menuName = "游戏设置/副本配置", fileName = "DungeonConfigData")]
    public class DungeonConfigData : SerializedScriptableObject
    {
        public List<LevelConfigData> levels = new List<LevelConfigData>();
#if UNITY_EDITOR
        [Button("保存该副本信息至文件")]
        public void SaveAsFile(ItemCollection itemCollection)
        {
            using (FileStream file = File.Create(Application.dataPath + "../../../Config/DungeonConfig/" + this.name + ".bytes")) 
            {                
                BinaryFormatter bf = new BinaryFormatter();
                //序列化
                bf.Serialize(file, levels);
                file.Close();
            }

        }
#endif
    }
}
