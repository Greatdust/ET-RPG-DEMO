using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Box2DSharp.Dynamics;
using ETModel;
using Sirenix.OdinInspector;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;


public class PTools 
{
    public const string clientPath = "/Res/MapData/";
    public const string serverPath = "../../../Config/PhysicWorlds/Map/";
    [MenuItem("2D物理/导出静态Box数据")]
    static void ExportAllStaticBody()
    {
        PBoxColliderHelper[] pBoxs = Selection.activeTransform.GetComponentsInChildren<PBoxColliderHelper>();
        List<PBoxData> pBoxDatas = new List<PBoxData>();
        foreach (var v in pBoxs)
        {
            if (v.bodyType == BodyType.StaticBody)
            {
                var data = new PBoxData()
                {
                    eulerAnglesY = v.transform.eulerAngles.y
                };
                data.offset.Fill(v.offset);
                data.pos.Fill(v.transform.position);
                data.size.Fill(v.size);
                pBoxDatas.Add(data);

            }
        }
        string clientFolderPath = Application.dataPath + clientPath ;
        Directory.CreateDirectory(clientFolderPath);


        Serialize(clientFolderPath + Selection.activeTransform.name + ".bytes", pBoxDatas);
        Log.Debug("保存至客户端成功!");
        string serverFolderPath = Application.dataPath + serverPath ;
        Directory.CreateDirectory(serverFolderPath);


        Serialize(serverFolderPath + Selection.activeTransform.name + ".bytes", pBoxDatas);
        Log.Debug("保存至服务器成功!");
    }

    static void Serialize(string path, object obj)
    {
        using (FileStream file = File.Create(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            //序列化
            bf.Serialize(file, obj);
            file.Close();
        }
    }

}
