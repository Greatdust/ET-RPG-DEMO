using ETModel;
using UnityEngine;

//执行BUFF时,获得的返回值,用以接下来执行BUFF时的输入
public interface IBufferValue
{

}

//
public struct BufferValue_Pos : IBufferValue
{
    public Vector3 startPos; // 使用者使用技能的起始位置
    public Vector3 aimPos; // 使用者选择的目标位置
}

//锁定类相关BUFF需要的输入,范围类技能命中多个目标时需要的输出
public struct BufferValue_TargetUnits : IBufferValue
{
    public Unit[] targets;
}

//方向
public struct BufferValue_Dir : IBufferValue
{
    public Vector3 dir;
}

//速度
public struct BufferValue_Speed : IBufferValue
{
    public float speed;
}

//伤害加成
public struct BufferValue_DamageAddPct : IBufferValue
{
    public float damageAddPct;
}

//一定暴击
public struct BufferValue_Crit : IBufferValue
{
    public bool isCrit;
}


//上述提供的基础类型可能不够用,如果要实现什么特殊效果,需要特殊的数据,就再添加好了.