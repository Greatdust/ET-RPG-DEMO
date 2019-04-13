using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.U2D;

[CreateAssetMenu(menuName = "游戏设置/全局设置", fileName = "GameGlobalConfig")]
public class GameGlobalConfig : SerializedScriptableObject
{
   
    [TabGroup("游戏基础配置")]
    [LabelText("游戏时间纪元前缀名")]
    public string GameEraPrefixName;
    [TabGroup("游戏基础配置")]
    [LabelText("初始游戏时间_年")]
    public int DateOnGameStart_Year;
    [TabGroup("游戏基础配置")]
    [LabelText("初始游戏时间_月")]
    public int DateOnGameStart_Month;
    [TabGroup("游戏基础配置")]
    [LabelText("初始游戏时间_天")]
    public int DateOnGameStart_Day;
    [TabGroup("游戏基础配置")]
    [LabelText("初始游戏时间_小时")]
    public int DateOnGameStart_Hour;
    [TabGroup("游戏基础配置")]
    [LabelText("游戏开始界面的背景音乐")]
    public AudioClip homePageBgAudio;

    [TabGroup("玩家角色基础配置")]
    [LabelText("背包格子最大数量")]
    public int MaxItemNumInInventory = 120;
    [TabGroup("玩家角色基础配置")]
    [LabelText("玩家角色初始寿命(天)")]
    public int PlayerLifeTimeOnGameStart = 180;
    [TabGroup("玩家角色基础配置")]
    [LabelText("玩家角色初始精力上限")]
    public int PlayerVigourMaxOnGameStart = 100;
    [TabGroup("玩家角色基础配置")]
    [LabelText("玩家角色精力成长值(随等级)")]
    public int PlayerVigourGrowthValue = 20;
    [TabGroup("玩家角色基础配置")]
    [LabelText("玩家副功法发挥效果")]
    public float valueForAuxiliaryGongFaUpTo = 0.5f;

    [TabGroup("玩家行为对应配置表序号")]
    public int X_休息2小时 = 1001;
    [TabGroup("玩家行为对应配置表序号")]
    public int X_休息4小时 = 1002;
    [TabGroup("玩家行为对应配置表序号")]
    public int S_睡觉 = 1003;
    [TabGroup("玩家行为对应配置表序号")]
    public int P_劈材挑水 = 1004;
    [TabGroup("玩家行为对应配置表序号")]
    public int X_巡视山门 = 1005;
    [TabGroup("玩家行为对应配置表序号")]
    public int D_锻炼身体 = 1006;
    [TabGroup("玩家行为对应配置表序号")]
    public int Q_切磋 = 1007;
    [TabGroup("玩家行为对应配置表序号")]
    public int Z_照看灵植 = 1008;

    [TabGroup("小游戏对应任务id")]
    [LabelText("劈材挑水")]
    public long G01QuestId = 1001;
    [TabGroup("小游戏对应任务id")]
    [LabelText("巡视山门")]
    public long G02QuestId = 1002;
    [TabGroup("小游戏对应任务id")]
    [LabelText("照看灵植")]
    public long G03QuestId = 1003;

    [TabGroup("道具基础图集")]
    [LabelText("基础图集")]
    public SpriteAtlas baseItemAtlas;
    [TabGroup("道具基础图集")]
    [LabelText("白色品质底图名")]
    public string itemQuality_Bai;
    [TabGroup("道具基础图集")]
    [LabelText("绿色品质底图名")]
    public string itemQuality_Lv;
    [TabGroup("道具基础图集")]
    [LabelText("蓝色品质底图名")]
    public string itemQuality_Lan;
    [TabGroup("道具基础图集")]
    [LabelText("粉色品质底图名")]
    public string itemQuality_Fen;
    [TabGroup("道具基础图集")]
    [LabelText("金色品质底图名")]
    public string itemQuality_Jin;
    [TabGroup("道具基础图集")]
    [LabelText("传奇品质底图名")]
    public string itemQuality_QC;

    //public Sprite GetItemQualitySprite(ItemQualityLevel itemQualityLevel)
    //{
    //    switch (itemQualityLevel)
    //    {
    //        case ItemQualityLevel.劣质:

    //            return baseItemAtlas.GetSprite(itemQuality_Bai);
    //        case ItemQualityLevel.下品:
    //            return baseItemAtlas.GetSprite(itemQuality_Lv);
    //        case ItemQualityLevel.中品:
    //            return baseItemAtlas.GetSprite(itemQuality_Lan);
    //        case ItemQualityLevel.上品:
    //            return baseItemAtlas.GetSprite(itemQuality_Fen);
    //        case ItemQualityLevel.极品:
    //            return baseItemAtlas.GetSprite(itemQuality_Jin);
    //        default:
    //            return null;
    //    }
    //}

}

