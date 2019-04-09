using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ObjectSystem]
public class EquipInventoryComponentAwakeSystem : AwakeSystem<EquipInventoryComponent>
{
    public override void Awake(EquipInventoryComponent self)
    {
        self.Awake();
    }
}

//玩家的Unit身上都需要有的一个装备栏组件,用以管理角色身上的装备. 敌人和宠物不需要
public class EquipInventoryComponent : ETModel.Component
{
    public List<EquipData> inventory;//

    public int equipMaxNum;

    public void Awake()
    {
        equipMaxNum = GlobalConfigComponent.Instance.globalConfig.MaxItemNumInInventory;
        inventory = new List<EquipData>();
        for (int j = 0; j < equipMaxNum; j++)
        {
            inventory.Add(null);
        }
    }

    public bool AddItemData(string typeId,out long itemId)
    {
        int nullIndex = 0;
        if (!CheckNullSpace(ref nullIndex))
        {
            Game.EventSystem.Run(EventIdType.EquipInventoryIsFull);
            itemId = 0;
            return false;
        }
        EquipData data = EquipConfigComponent.instance.GetDeepCopy(typeId);
        itemId = data.itemId;
        data.buffGroup = new BuffGroup();
        for (int i = data.attrList.Count-1; i >= 0; i--)
        {
            if (RandomHelper.RandomNumber(0, 100) / 100.0f > data.attrList[i].chance)
            {
                data.attrList.Remove(data.attrList[i]);
                continue;
            }
            float pct = RandomHelper.RandomNumber((int)(100 * data.attrList[i].effectLowerLimit), 100) / 100.0f;
            if (pct < 1)
            {
                if (Mathf.Abs(data.attrList[i].value) >= 1)
                    data.attrList[i].value = Mathf.RoundToInt(data.attrList[i].value * pct);
                else
                    data.attrList[i].value = Mathf.RoundToInt(data.attrList[i].value * pct * 100) / 100.0f;
            }
            Buff_UpdateNumeric buff_UpdateNumeric = BuffFactory.CreateBuff(typeof(Buff_UpdateNumeric)) as Buff_UpdateNumeric;
            buff_UpdateNumeric.valueAdd = data.attrList[i].value;
            buff_UpdateNumeric.targetNumeric = data.attrList[i].attrType;
            data.buffGroup.AddBuff(buff_UpdateNumeric);
        }

       

        inventory[nullIndex] = data;
        Game.EventSystem.Run(EventIdType.EquipInventoryUpdated, nullIndex);
        return true;

    }
    public bool AddItemData(EquipData itemData)
    {
        if (itemData == null) return false;
        int nullIndex = 0;
        if (!CheckNullSpace(ref nullIndex))
        {
            Game.EventSystem.Run(EventIdType.EquipInventoryIsFull);
            return false;
        }
        
        inventory[nullIndex] = itemData;
        Game.EventSystem.Run(EventIdType.EquipInventoryUpdated, nullIndex);
        return true;

    }

    public EquipData GetEquipData(long itemId)
    {
        return inventory.Find(v => v.itemId == itemId);
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

    public void SortInventory()
    {
        inventory.OrderBy(t => t.itemName);
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

    public void ExchangeEquip(int source, int dest)
    {
        EquipData temp = inventory[source];
        inventory[source] = inventory[dest];
        inventory[dest] = temp;
    }

    public void Equip(long itemId)
    {
        int index = inventory.FindIndex(t => t != null ? t.itemId == itemId : false);
        if (index < 0) return;
        var data = inventory[index];
        EquipmentComponent equipmentComponent = GetParent<Unit>().GetComponent<EquipmentComponent>();
        inventory[index] = equipmentComponent.UpdateEquip(data.equipType, data);
        Game.EventSystem.Run(EventIdType.EquipInventoryUpdated, index);
    }

    public void RemoveEquip(long itemId)
    {
        int index = inventory.FindIndex(t => t != null ? t.itemId == itemId : false);
        if (index < 0) return;
        inventory[index] = null;
        Game.EventSystem.Run(EventIdType.EquipInventoryUpdated, index);
    }
}

