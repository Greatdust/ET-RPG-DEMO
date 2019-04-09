using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class ItemConfigEditor :EditorWindow
{
    private const string collectionFullPath = "Assets/Res/GameConfig/ItemConfig/ItemCollection.asset";

    [MenuItem("配置工具/道具配置")]
    static void ShowWindow()
    {
        if (!File.Exists(collectionFullPath))
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<ItemCollection>(), collectionFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(collectionFullPath);
    } 

}

