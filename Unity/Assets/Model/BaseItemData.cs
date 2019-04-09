using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;
using UnityEngine.U2D;

public enum ItemQualityLevel
{
    劣质,//白色
    下品,//蓝色
    中品,//青色
    上品,//紫色
    极品 //金色
}


[Serializable]
public abstract class BaseItemData
{
    [FoldoutGroup("道具基本信息")]
    [LabelText("类型id")]
    public string typeId;//类型的Id,作为同样物品的标志
    [FoldoutGroup("道具基本信息")]
    [LabelText("道具名称")]
    public string itemName;
    [FoldoutGroup("道具基本信息")]
    [LabelText("道具描述")]
    public string description;

    [NonSerialized]
    
    public Sprite icon;

    public UnityEngine.Sprite GetSprite()
    {
        if (string.IsNullOrEmpty(iconName))
        {
            UnityEngine.Debug.LogError("物品图标为空!");
            return null;
        }
        if (icon != null)
        {
            return icon;
        }
        string abName = spriteAtlasName + ".unity3d";
        Game.Scene.GetComponent<ResourcesComponent>().LoadBundle(abName);
        UnityEngine.U2D.SpriteAtlas spriteAtlas = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(abName, spriteAtlasName) as UnityEngine.U2D.SpriteAtlas;   
        icon = spriteAtlas.GetSprite(iconName);
        if (icon == null)
        {
            Debug.LogError("获得Sprite为空,请检查物品名:"+iconName);
        }
        return icon;

    }
    [FoldoutGroup("道具基本信息/图片相关")]
    [LabelText("道具图集名称")]
    public string spriteAtlasName;
    [FoldoutGroup("道具基本信息/图片相关")]
    [LabelText("道具图片名称")]
    public string iconName;
#if UNITY_EDITOR
    [FoldoutGroup("道具基本信息/图片相关")]
    [ReadOnly]
    [PreviewField(ObjectFieldAlignment.Center)]
    [NonSerialized]
    [ShowInInspector]
    public Sprite preview;
    [FoldoutGroup("道具基本信息/图片相关")]
    [Button("预览图片",ButtonSizes.Medium)]
    public void Display()
    {

        if (string.IsNullOrEmpty(spriteAtlasName) || string.IsNullOrEmpty(iconName))
        {
            return;
        }
        else
        {
            string[] AssetsPath = AssetDatabase.GetAssetPathsFromAssetBundle(spriteAtlasName.ToLower() + ".unity3d");
            if (AssetsPath == null)
            {
                return;
            }
            else
            {
                preview = (AssetDatabase.LoadMainAssetAtPath(AssetsPath[0]) as SpriteAtlas).GetSprite(iconName);
            }
        }

    }
#endif
}



