using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[Serializable]
public abstract class BaseItemWithMulAttr : BaseItemData
{
    [HideInInspector]
    public long itemId;//唯一,每个item都是单独的
    [FoldoutGroup("道具基本信息")]
    [LabelText("售出价格")]
    public int salePrice;
    [FoldoutGroup("道具基本信息")]
    [LabelText("道具品质")]
    public ItemQualityLevel itemQualityLevel;
    [FoldoutGroup("道具基本信息")]
    [LabelText("道具使用条件")]
    public List<IItemUseCondition> itemUseConditions;
    public void AddUseCondition(IItemUseCondition itemUseCondition)
    {
        if (itemUseConditions == null)
            itemUseConditions = new List<IItemUseCondition>();
        itemUseConditions.Add(itemUseCondition);
    }
}



