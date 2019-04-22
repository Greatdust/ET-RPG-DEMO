using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public enum BuffTargetType
{
    自身,
    范围内我方角色,
    范围内敌方角色,
    敌方单体,
}

public enum RestrictionType
{
    击退,
    眩晕,
}

/// <summary>
/// 如果这个BUFF是叠加的,当有相同类型,比如同种DOT,同种效果的BUFF,那么重复施放的效果就是叠加BUFF层数
/// 刷新就是刷新时间
/// 独立就是该BUFF和其他BUFF是分开的
/// </summary>
public enum BuffStackType
{
    叠加,
    刷新,
    独立
}
[Serializable]

[MessagePack.Union(0, typeof(Buff_AddBuff))]
[MessagePack.Union(1, typeof(Buff_CostHP_MP))]
[MessagePack.Union(2, typeof(Buff_DamageByNumeric))]
[MessagePack.Union(3, typeof(Buff_DirectDamage))]
[MessagePack.Union(4, typeof(Buff_DOT))]
[MessagePack.Union(5, typeof(Buff_EmitObj))]
[MessagePack.Union(6, typeof(Buff_EnhanceSkillEffect))]
[MessagePack.Union(7, typeof(Buff_GiveNumeric))]
[MessagePack.Union(8, typeof(Buff_GiveRecover))]
[MessagePack.Union(9, typeof(Buff_GiveSpecialDebuff))]
[MessagePack.Union(10, typeof(Buff_HitEffect))]
[MessagePack.Union(11, typeof(Buff_Move))]
[MessagePack.Union(12, typeof(Buff_PlayAnim))]
[MessagePack.Union(13, typeof(Buff_PlayEffect))]
[MessagePack.Union(14, typeof(Buff_PlaySound))]
[MessagePack.Union(15, typeof(Buff_PushBack))]
[MessagePack.Union(16, typeof(Buff_RangeDetection))]
[MessagePack.Union(17, typeof(Buff_UpdateNumeric))]

public abstract class BaseBuffData
{
    //这个string值的意义,是为了可能需要做的,将技能数据层放到热更代码里
    //这样热更的时候修改数值(平衡性更新,客户端一般只改变描述,实际计算在服务器) 完全不影响技能的执行
    public abstract string GetBuffIdType();
    [ReadOnly]
    [LabelText("SIGNAL")]
    [LabelWidth(100)]
    public string buffSignal;//一个BUFF和其他BUFF区别开的标志,所有BUFF彼此之间,这个id都是唯一的

    public BaseBuffData()
    {
        //需要保证每个技能的每个buffSignal 都是唯一的
        buffSignal = "B_" + GetBuffIdType() +"_"+ IdGenerater.GenerateId().ToString();
    }
}


