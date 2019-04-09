using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ObjectSystem]
public class EquipmentComponentAwakeSystem : AwakeSystem<EquipmentComponent>
{
    public override void Awake(EquipmentComponent self)
    {
        self.Awake();
    }
}

//玩家的Unit身上都需要有的一个装备组件,用以管理角色身上的装备. 敌人和宠物不需要,直接配置对应属性就好
public class EquipmentComponent : ETModel.Component
{
    public Dictionary<int, EquipData> equipDataDic;//每个Type都对应一件装备到身上的装备


    public void Awake()
    {
        equipDataDic = new Dictionary<int, EquipData>();
    }

    public EquipData GetEquipData(EquipPosType equipType)
    {
        EquipData equipData;
        equipDataDic.TryGetValue((int)equipType, out equipData);
        return equipData;
    }

    public EquipData UpdateEquip(EquipPosType equipType, EquipData newEquipData)
    {
        EquipData data = UnEquip(equipType);
        equipDataDic[(int)equipType] = newEquipData;
   
        Equip(newEquipData);
        Game.EventSystem.Run(EventIdType.EquipUpdated);
        return data;
    }

    public EquipData UnEquip(EquipPosType equipType)
    {
        EquipData data;
        equipDataDic.TryGetValue((int)equipType, out data);
        if (data == null)
        {
            return data;
        }
        equipDataDic[(int)equipType] = null;
        BuffMgrComponent buffMgrComponent = GetParent<Unit>().GetComponent<BuffMgrComponent>();

        buffMgrComponent.RemoveGroup(data.buffGroup.BuffGroupId);
        Game.EventSystem.Run(EventIdType.EquipUpdated);
        return data;
    }

    void Equip(EquipData baseEquipData)
    {
        BuffMgrComponent buffMgrComponent = GetParent<Unit>().GetComponent<BuffMgrComponent>();

        Log.Debug("添加Buff" + baseEquipData.buffGroup.BuffGroupId);
        buffMgrComponent.AddBuffGroup(baseEquipData.buffGroup.BuffGroupId, baseEquipData.buffGroup);
    }
}

