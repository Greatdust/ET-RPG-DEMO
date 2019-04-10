using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IItemUseCondition
{
    ItemUseConditionType GetItemUseConditionType();
    bool MeetCondition();
}
public enum ItemUseConditionType
{
    等级限制
}

public enum ItemType
{
    消耗品,
    特殊道具,
    材料,
    功法
}
[Serializable]
public class ItemData : BaseItemWithMulAttr
{
    public ItemType itemType;//Item的类型
    public int num;//叠加的数量
    public int maxNum;//叠加数量上限
    public BuffGroup buffGroup;


    public void AddBuff(BaseBuffData data)
    {
        if (buffGroup == null)
            buffGroup = new BuffGroup();
        buffGroup.AddBuff(data);
    }
}


[Serializable]
public class ItemUseCondition_LevelLimit : IItemUseCondition
{
    [LabelText("道具使用等级限制")]
    [LabelWidth(200)]
    public int limitLevel;

    public ItemUseConditionType GetItemUseConditionType()
    {
        return ItemUseConditionType.等级限制;
    }

    public bool MeetCondition()
    {
        //TODO: 玩家等级达到要求
        NumericComponent numericComponent = UnitComponent.Instance.MyUnit.GetComponent<NumericComponent>();
        if (numericComponent.GetAsInt(NumericType.Level) >= limitLevel)

            return true;
        else
            return false;
    }
}


