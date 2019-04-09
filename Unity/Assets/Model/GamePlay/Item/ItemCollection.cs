using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Google.Protobuf;
using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;

[Serializable]
public class ItemCollection : SerializedScriptableObject
{
    public Dictionary<string, ItemData> itemDic = new Dictionary<string, ItemData>();

    public static void SaveAsFile(ItemCollection itemCollection)
    {
        using (FileStream file = File.Create(Application.dataPath+"/../pbdata.data"))
        {
            List<ItemData> itemDatas = itemCollection.itemDic.Values.ToList();
            BinaryFormatter bf = new BinaryFormatter();
            //序列化
            bf.Serialize(file, itemDatas);

            file.Close();
        }

    }

    public static void Deserializer()
    {
        using (FileStream fileStream = File.OpenRead(Application.dataPath + "/../pbdata.data"))
        {
            //二进制格式化程序
            BinaryFormatter bf = new BinaryFormatter();
            //反序列化
            List<ItemData> user = (List<ItemData>)bf.Deserialize(fileStream);
            Debug.Log(user[0].description);
            fileStream.Close();
        }
    }
    
}

