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
    public const string serverPath = "../../../Config/PhysicWorlds/";
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
        string serverFolderPath = Application.dataPath + serverPath+ "Map/" ;
        Directory.CreateDirectory(serverFolderPath);


        Serialize(serverFolderPath + Selection.activeTransform.name + ".bytes", pBoxDatas);
        Log.Debug("保存至服务器成功!");
    }

    [MenuItem("2D物理/导出文件夹内所有非静态物体")]
    static void ExportAllNONStaticBody()
    {
        string fullPath = AssetDatabase.GetAssetPath(Selection.activeObject);
        Debug.Log(fullPath);

        //这里根据物体名字来记录对应的PBox数据
        Dictionary<string, PBoxData> boxDataDic = new Dictionary<string, PBoxData>();
        Dictionary<string, PCircleData> circleDataDic = new Dictionary<string, PCircleData>();


        if (Directory.Exists(fullPath))
        {
            DirectoryInfo dirInfo = new DirectoryInfo(fullPath);
            FileInfo[] files = dirInfo.GetFiles("*.prefab"); //包括子目录
            Debug.Log(files.Length);
            for (int i = 0; i < files.Length; i++)
            {
                if (files[i].Name.EndsWith(".prefab"))
                {
                    GameObject obj = AssetDatabase.LoadAssetAtPath(fullPath +"/"+files[i].Name,typeof(GameObject)) as GameObject;

                    PBoxColliderHelper pBoxCollider = obj.GetComponentInChildren<PBoxColliderHelper>();
                    if (pBoxCollider == null)
                    {
                        PCircleColliderHelper pCircleCollider = obj.GetComponentInChildren<PCircleColliderHelper>();
                        if (pCircleCollider == null) continue;

                        PCircleData pCircleData = new PCircleData()
                        {
                            eulerAnglesY = pCircleCollider.transform.eulerAngles.y,
                             bodyType = pCircleCollider.bodyType
                        };
                        pCircleData.offset.Fill(pCircleCollider.offset);
                        pCircleData.halfHeight = pCircleCollider.height;
                        pCircleData.radius = pCircleCollider.radius;
                        string key = files[i].Name.Replace(".prefab", "");
                        Debug.Log("Key  " + key);
                        circleDataDic.Add(key, pCircleData);
                    }
                    else
                    {
                        PBoxData pBoxData = new PBoxData()
                        {
                            eulerAnglesY = pBoxCollider.transform.eulerAngles.y,
                            bodyType = pBoxCollider.bodyType
                        };
                        pBoxData.offset.Fill(pBoxCollider.offset);
                        pBoxData.pos.Fill(Vector3.zero); // 因为是非静态物体,所以这里记录位置没用,统一用zero
                        pBoxData.size.Fill(pBoxCollider.size);
                        string key = files[i].Name.Replace(".prefab", "");
                        Debug.Log("Key  " + key);
                        boxDataDic.Add(key, pBoxData);
                    }
                }
            }
        }
        else
        {
            Debug.Log("资源路径不存在");
        }
        string boxsFolder = Application.dataPath + serverPath + "BoxsData/";
        Directory.CreateDirectory(boxsFolder);
        string circleFolder = Application.dataPath + serverPath + "CirclesData/";
        Directory.CreateDirectory(circleFolder);
        if (boxDataDic.Count > 0)
        {
            Serialize(boxsFolder + Selection.activeObject.name + ".bytes", boxDataDic);
        }
        if (circleDataDic.Count > 0)
        {
            Serialize(circleFolder + Selection.activeObject.name + ".bytes", circleDataDic);
        }
        Log.Debug("导出成功!");
    }

    static void Serialize<T>(string path, T obj)
    {

        var bin = MessagePack.MessagePackSerializer.Serialize(obj, MessagePack.Resolvers.ContractlessStandardResolver.Instance);
        File.WriteAllBytes(path, bin);


    }

}
