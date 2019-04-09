using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ObjectSystem]
public class ItemInventoryComponentAwakeSystem : AwakeSystem<ItemInventoryComponent>
{
    public override void Awake(ItemInventoryComponent self)
    {
        self.Awake();
    }
}

//玩家的Unit身上都需要有的一个道具栏组件,用以管理角色身上的道具. 敌人和宠物不需要,直接配置对应属性就好
public class ItemInventoryComponent : ETModel.Component
{
    public List<ItemData> inventory;//一个序号对应一个格子
    //TODO:池化?还是预先创建好所有的data
    [NonSerialized]
    public Dictionary<string, List<ItemData>> itemTypeDic; //一种类型对应一组道具

    public int itemMaxNum;

    public void Awake()
    {
        itemMaxNum = GlobalConfigComponent.Instance.globalConfig.MaxItemNumInInventory;
        itemTypeDic = new Dictionary<string, List<ItemData>>();
        inventory = new List<ItemData>(itemMaxNum);
        for (int i = 0; i < itemMaxNum; i++)
        {
            inventory.Add(null);
        }
    }

    public bool AddItemData(string typeId, int num)
    {
        int MaxNum = ItemConfigComponent.instance.GetMaxNumFromTypeId(typeId);
        int newDataNum = 0;
        if (num >= MaxNum)
        {
            int newDataCount = MaxNum / num;
            newDataNum = MaxNum % num;
            for (int i = 0; i < newDataCount; i++)
            {
                ItemData data = ItemConfigComponent.instance.GetDeepCopy(typeId);
                data.num = MaxNum;
                if (!AddNewItem(data))
                {
                    return false;
                }
            }
        }
        else
        {
            newDataNum = num;
        }
        List<ItemData> list;
        if (itemTypeDic.TryGetValue(typeId, out list))
        {
            bool hasAdded = false;
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].num == MaxNum)
                {
                    continue;
                }
                else
                {
                    list[i].num += newDataNum;
                    hasAdded = true;
                    Game.EventSystem.Run(EventIdType.ItemInventoryUpdated, inventory.IndexOf(list[i]));
                    if (list[i].num > MaxNum)
                    {
                        ItemData data = ItemConfigComponent.instance.GetDeepCopy(typeId);
                        data.num = list[i].num - MaxNum;
                        list[i].num = MaxNum;
                        if (!AddNewItem(data))
                        {
                            return false;
                        }
                    }
                    break;
                }
            }
            if (!hasAdded)
            {
                ItemData data = ItemConfigComponent.instance.GetDeepCopy(typeId);
                data.num = newDataNum;
                if (!AddNewItem(data))
                {
                    return false;
                }
                hasAdded = true;
            }

        }
        else
        {
            ItemData data = ItemConfigComponent.instance.GetDeepCopy(typeId);
            data.num = num;
            if (!AddNewItem(data))
            {
                return false;
            }
        }
        return true;

    }
    bool AddNewItem(ItemData itemData)
    {
        int nullIndex = 0;
        if (!CheckNullSpace(ref nullIndex))
        {
            Game.EventSystem.Run(EventIdType.ItemInventoryIsFull);
            return false;
        }
        inventory[nullIndex] = itemData;
        Game.EventSystem.Run(EventIdType.ItemInventoryUpdated, nullIndex);
        List<ItemData> list;
        if (!itemTypeDic.TryGetValue(itemData.typeId, out list))
        {
            list= new List<ItemData>(); 
            itemTypeDic[itemData.typeId] = list;

        }
        list.Add(itemData);
        return true;
    }

    bool CheckNullSpace(ref int index)
    {
        for (int i = 0; i < inventory.Count; i++)
        {
            if (inventory[i] == null)
            {
                index = i;
                return true;
            }
        }
        return false;
    }

    public int GetItemNumByType(string typeId)
    {
        List<ItemData> itemDatas;
        if (!itemTypeDic.TryGetValue(typeId, out itemDatas))
        {
            return 0;
        }
        int num = 0;
        foreach (var v in itemDatas)
        {
            num += v.num;
        }
        return num;
    }


    public void SortInventory()
    {
        inventory.OrderBy(t => t.typeId);
    }

    public int GetInventoryNum()
    {
        int num = 0;
        foreach (var v in inventory)
        {
            if (v != null)
                num++;
        }
        return num;
    }


    public void ExchangeItem(int source, int dest)
    {
        ItemData temp = inventory[source];
        inventory[source] = inventory[dest];
        inventory[dest] = temp;
        Game.EventSystem.Run(EventIdType.ItemInventoryUpdated, source);
        Game.EventSystem.Run(EventIdType.ItemInventoryUpdated, dest);
    }

    public void UseItem(int index, long[] targets)
    {
        var data = inventory[index];
        foreach (var v in data.itemUseConditions)
        {
            if (!v.MeetCondition())
                return;//TODO:这里应该抛个事件,提示使用条件不满足k
        }

        BuffMgrComponent buffMgrComponent = GetParent<Unit>().GetComponent<BuffMgrComponent>();
        buffMgrComponent.AddBuffGroup(data.buffGroup.BuffGroupId, data.buffGroup);
        data.num -= 1;
        if (data.num < 0)
        {
            RemoveItem(index);
        }
        else
        {
            Game.EventSystem.Run(EventIdType.ItemInventoryUpdated, index);
        }
    }
    public void UseItem(long itemId)
    {
        int index = inventory.FindIndex(t => t != null ? t.itemId == itemId : false);
        if (index < 0) return;
        var data = inventory[index];
        foreach (var v in data.itemUseConditions)
        {
            if (!v.MeetCondition())
                return;//TODO:这里应该抛个事件,提示使用条件不满足k
        }

        BuffMgrComponent buffMgrComponent = GetParent<Unit>().GetComponent<BuffMgrComponent>();
        buffMgrComponent.AddBuffGroup(data.buffGroup.BuffGroupId, data.buffGroup);
        data.num -= 1;
        if (data.num < 0)
        {
            RemoveItem(index);
        }
        else
        {
            Game.EventSystem.Run(EventIdType.ItemInventoryUpdated, index);
        }
    }


    public void RemoveItem(long itemId)
    {
        int index = inventory.FindIndex(t => t != null ? t.itemId == itemId : false);
        if (index < 0) return;
        RemoveItem(index);
    }
    void RemoveItem(int index)
    {
        inventory[index] = null;
        Game.EventSystem.Run(EventIdType.ItemInventoryUpdated, index);
    }

}

