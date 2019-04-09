using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum EquipPosType
{

    武器,

    胸部防具,

    脚部防具,

    腿部防具,

    项链,

    腰带,

    戒指
}


[Serializable]
public class EquipData : BaseItemWithMulAttr
{
    [LabelText("装备部位类型")]
    [LabelWidth(150)]
    public EquipPosType equipType;
    [LabelText("装备耐久度")]
    [LabelWidth(150)]
    public int endurance = 30;//耐久度
    [LabelText("装备道具等级(装等)")]
    [LabelWidth(150)]
    public int itemLevel = 1;//装备品级,简称装等
    [HideInInspector]
    public BuffGroup buffGroup;
    
    public List<EquipAttribute> attrList;

    /// <summary>
    /// 为了给装备的属性制造一点随机性
    /// </summary>
    [Serializable]
    public class EquipAttribute
    {
        [LabelText("属性类型")]
        [LabelWidth(150)]
        public NumericType attrType;
        [LabelText("属性值")]
        [LabelWidth(150)]
        public float value;
        [LabelText("属性出现的几率")]
        [LabelWidth(150)]
        public float chance = 1;//出现的概率,0~1
        [LabelText("属性实际效果下限")]
        [LabelWidth(150)]
        public float effectLowerLimit = 1;//效果的下限,0~1,一般创建装备的时候,根据设定的标准值,在这个下限值和1之间取一个float值,和标准值相乘,得到这条属性的实际值
    }

    [Serializable]
    public struct EquipPassiveSkillData
    {
        [LabelText("被动技能Id")]
        [LabelWidth(150)]
        public string skillId;
    }
    [LabelText("装备携带被动技能列表")]
    [LabelWidth(150)]
    public List<EquipPassiveSkillData> passiveSkillDatas = new List<EquipPassiveSkillData>();//装备携带的特殊效果

    public void AddEquipAttribute(EquipAttribute ep)
    {
        if (attrList == null)
            attrList = new List<EquipAttribute>();
        attrList.Add(ep);
    }
}

public enum WeaponType
{
    剑,
    笔
}
[Serializable]
public class WeaponData : EquipData
{
    public WeaponType weaponType;
}
