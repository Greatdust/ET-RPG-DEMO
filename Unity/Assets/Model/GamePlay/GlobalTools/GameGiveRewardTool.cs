using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[Event(EventIdType.GiveItem)]
public class Reward_GiveItemEvent : AEvent<Dictionary<string, int>>
{
    public override void Run(Dictionary<string, int> dic)
    {
        foreach(var v in dic)
        GameGiveRewardTool.GiveItem(v.Key, v.Value);
    }
}

[Event(EventIdType.GiveEquip)]
public class Reward_GiveEquipEvent : AEvent<List<string>>
{
    public override void Run(List<string> list)
    {
        for (int i = 0; i < list.Count; i++)
        {
            GameGiveRewardTool.GiveEquip(list[i],out long itemId);
        }
    }
}


public static class GameGiveRewardTool
{
    
    public static void GiveItem(string typeId, int num)
    {
        if (string.IsNullOrEmpty(typeId))
        {
            return;
        }
        Unit mUnit = UnitComponent.Instance.MyUnit;
        ItemInventoryComponent itemInventoryComponent = mUnit.GetComponent<ItemInventoryComponent>();
        Debug.LogFormat("玩家获得道具{0},数量为{1}", typeId, num);
        itemInventoryComponent.AddItemData(typeId, num);
    }
    public static void GiveEquip(string typeId,out long itemId)
    {
        if (string.IsNullOrEmpty(typeId))
        {
            itemId = 0;
            return;
        }
        Unit mUnit = UnitComponent.Instance.MyUnit;
        EquipInventoryComponent itemInventoryComponent = mUnit.GetComponent<EquipInventoryComponent>();
        Debug.LogFormat("玩家获得装备 {0}", typeId);
        itemInventoryComponent.AddItemData(typeId,out itemId);
    }


    public static void GiveExp(Unit unit,int num,out float preProgress,out float currProgress,out bool lvUp)
    {
        NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
        if (numericComponent.GetAsInt(NumericType.等级) == numericComponent.GetAsInt(NumericType.等级Max))
        {
            preProgress = 1;
            currProgress = 1;
            lvUp = false;
            return;
        }
        Debug.LogFormat("单位{0}获得经验 {1}", unit.Id, num);
        int expMax = numericComponent.GetAsInt(NumericType.经验Max);
        int preExp = numericComponent.GetAsInt(NumericType.经验);
        preProgress = preExp / ((float)expMax);
        int currExp = preExp + num;
        lvUp = false;
        if (currExp >= expMax)
        {
            numericComponent.Set(NumericType.等级, numericComponent.GetAsInt(NumericType.等级) + 1);
            lvUp = true;
            Game.EventSystem.Run(EventIdType.UnitLvUp, unit.Id, numericComponent.GetAsInt(NumericType.等级));
            if (numericComponent.GetAsInt(NumericType.等级) == numericComponent.GetAsInt(NumericType.等级Max))
            {
                currProgress = 1;
                return;
            }
            currExp -= expMax;
            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();
            //ExpForLevelUp expConfig = configComponent.Get(typeof(ExpForLevelUp), numericComponent.GetAsInt(NumericType.等级)) as ExpForLevelUp;
            //expMax = expConfig.Exp;
            //numericComponent.Set(NumericType.经验Max, expMax);
        }
        currProgress = Mathf.Clamp01(currExp / ((float)expMax));
        numericComponent.Set(NumericType.经验, currExp);

    }

    public static void GiveNumeric(NumericType numericType, float value)
    {
        Unit mUnit = UnitComponent.Instance.MyUnit;
        NumericComponent numericComponent = mUnit.GetComponent<NumericComponent>();
        numericComponent.Set(numericType, numericComponent.GetAsFloat(numericType) + value);
    }
}

