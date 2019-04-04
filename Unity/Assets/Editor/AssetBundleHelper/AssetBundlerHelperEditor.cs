using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using ETModel;

public class AssetBundleBuilder: EditorWindow
{

    //资源存放路径前缀
    static string assetsDir ;
    //打包后存放路径
    const string assetBundlesPath = "Assets/AssetBundleBuilder/AssetBundles";

    [MenuItem("自动化工具/自动设置文件夹下所有资源的AB名")]
    public static void AutoName()
    {
        EditorWindow.GetWindow<AssetBundleBuilder>(false, "AB名设置", true);
        Debug.Log("设置完成");
    }

    private void OnEnable()
    {
        assetsDir = AssetDatabase.GetAssetPath(Selection.activeObject);
    }

    private void OnGUI()
    {
        assetsDir = EditorGUILayout.TextField("文件夹相对路径", assetsDir);
        if (GUILayout.Button("设置包名", GUILayout.Width(200)))
        {
            SetAssetBundlesName(assetsDir);
        }
    }

    /// <summary>
    /// 设置所有在指定路径下的AssetBundleName
    /// </summary>
    static void SetAssetBundlesName(string _assetsPath)
    {
        //先获取指定路径下的所有Asset，包括子文件夹下的资源
        DirectoryInfo dir = new DirectoryInfo(_assetsPath);
        FileSystemInfo[] files = dir.GetFileSystemInfos(); //GetFileSystemInfos方法可以获取到指定目录下的所有文件以及子文件夹

        for (int i = 0; i < files.Length; i++)
        {
            if (files[i] is DirectoryInfo)  //如果是文件夹则递归处理
            {
                SetAssetBundlesName(files[i].FullName);
            }
            else if (!files[i].Name.EndsWith(".meta")) //如果是文件的话，则设置AssetBundleName，并排除掉.meta文件
            {
                SetABName(files[i].FullName);     //逐个设置AssetBundleName
            }
        }

    }

    /// <summary>
    /// 设置单个AssetBundle的Name
    /// </summary>
    /// <param name="filePath"></param>
    static void SetABName(string assetPath)
    {
        string importerPath = "Assets" + assetPath.Substring(Application.dataPath.Length);  //这个路径必须是以Assets开始的路径
        AssetImporter assetImporter = AssetImporter.GetAtPath(importerPath);  //得到Asset


        string tempName = assetPath.Substring(assetPath.LastIndexOf(@"\") + 1);
        string assetName = tempName.Remove(tempName.LastIndexOf(".")); //获取asset的文件名称
        assetImporter.assetBundleName = assetName.StringToAB();    //最终设置assetBundleName
    }

}