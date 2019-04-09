using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;
using ETModel;
using UnityEditorInternal;

public class EquipConfigEditor : EditorWindow
{
    private const string collectionFullPath = "Assets/Res/GameConfig/EquipConfig/EquipCollection.asset";
    [MenuItem("配置工具/装备配置")]
    static void ShowWindow()
    {
        if (!File.Exists(collectionFullPath))
        {
            AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<EquipCollection>(), collectionFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        Selection.activeObject = AssetDatabase.LoadMainAssetAtPath(collectionFullPath);
    }
             
}

