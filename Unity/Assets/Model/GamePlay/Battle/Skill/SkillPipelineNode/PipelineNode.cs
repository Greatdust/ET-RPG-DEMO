using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BaseSkillData;


public enum Pipeline_TriggerType
{
    碰撞检测,//该项的所有Buff被触发的条件是上一个的所有BUFF触发后,经过几个特殊的,比如碰撞的事件后,触发该项
    固定时间,//一定时间后触发,本身又持续一定的时间
    循环开始,//用以控制后续多少个pipeline节点重复执行多少次.主要目的是实现盖伦E,闪电链,暴风雪这种持续伤害的技能
    循环结束, // 标记循环结束的节点
    等待用户输入,  //这个目的是为了接收用户输入的数据,传输到后续Pipeline中. 比如选择一个目标/方向/范围使用技能,蓄力类技能,引导类技能等,
    自动寻找目标, //这个目的是为了处理一些BUFF直接作用于自身,以自身为中心的范围内的目标,或者敌方队伍全体,我方队伍全体这种情况
    可编程,// 这个目的是暴露全部可以支持的Buff作为借口供外部脚本使用.
    条件判断 // 用以实现技能执行时满足一些条件执行A,满足另一些条件执行B的情况.比如一个技能的效果是发射一枚火球,但是有30%的概率连续发射两枚,或者周围环境是火焰环境,火球变成火龙
     
}

[Serializable]
public abstract class BasePipelineData
{
    public abstract Pipeline_TriggerType GetTriggerType();
    [LabelText("初始启用")]
    [LabelWidth(100)]
    public bool enable = true;//初始是否是启用状态
    [LabelText("SIGNAL")]
    [LabelWidth(100)]
    [ReadOnly]
    public string pipelineSignal;

    public BasePipelineData()
    {
        //需要保证每个技能的每个pipelineSignal 都是唯一的
        pipelineSignal = "P_" + IdGenerater.GenerateId().ToString();
    }
}



[Serializable]
public abstract class PipelineDataWithBuff : BasePipelineData
{
    [GUIColor(142 / 255.0f, 212 / 255.0f, 243/255.0f, 1f)]
    public List<BuffInSkill> buffs = new List<BuffInSkill>();

}

[LabelText("固定时间")]
[LabelWidth(150)]
[Serializable]
public class Pipeline_FixedTime : PipelineDataWithBuff
{
    [LabelText("延迟执行时间")]
    [LabelWidth(100)]
    public float delayTime;//等待多少秒后执行
    [LabelText("节点总共时间")]
    [LabelWidth(100)]
    public float fixedTime;//该阶段一共持续多少秒

    public override Pipeline_TriggerType GetTriggerType()
    {
        return Pipeline_TriggerType.固定时间;
    }
}

[LabelText("碰撞事件")]
[LabelWidth(150)]
[GUIColor(133 / 255.0f, 250 / 255.0f, 103 / 255.0f, 1f)]
[Serializable]
public class Pipeline_Collision : PipelineDataWithBuff
{
    public override Pipeline_TriggerType GetTriggerType()
    {
        return Pipeline_TriggerType.碰撞检测;
    }
}

[LabelText("循环开始")]
[LabelWidth(150)]
[Serializable]
public class Pipeline_CycleStart : BasePipelineData
{
    [LabelText("执行次数")]
    [LabelWidth(120)]
    public byte repeatCount = 1;//循环执行几次

    //TODO: 如果要考虑盖伦E这种技能(随着英雄等级导致技能的伤害频率提升的),这里还要加几个成长值. 但是DEMO的话无所谓了

    public override Pipeline_TriggerType GetTriggerType()
    {
        return Pipeline_TriggerType.循环开始;
    }
}

[LabelText("循环结束")]
[LabelWidth(150)]
[Serializable]
public class Pipeline_CycleEnd : BasePipelineData
{
    public override Pipeline_TriggerType GetTriggerType()
    {
        return Pipeline_TriggerType.循环结束;
    }
}

public enum InputType
{
    Tar, // 单个目标,锁定类 返回一个目标.
    Dir, // 方向,范围技能 返回一个方向, 
    Pos, // 地点,范围技能, 返回一个地点,
    Charge, // 蓄力类型的技能,返回一个伤害加成和方向
    Spell, // 引导类技能,需要读条
    ContinualSpell, // 持续引导类技能, 鼠标输入,即是方向 . 类似于这样的技能比较特殊,如果麻烦,就用可编程节点来解决好了.
}

public struct ChargeData
{
    [LabelText("阶段完成时间")]
    [LabelWidth(120)]
    public float duration;
    [LabelText("伤害加成")]
    [LabelWidth(120)]
    public float coff;
}

[LabelText("接收玩家输入")]
[LabelWidth(150)]
[Serializable]
public class Pipeline_WaitForInput : BasePipelineData
{
    public InputType inputType;

    // 如果是蓄力类型的,那么会有一个列表,代表蓄力达到第几段了,每一段增加多少伤害
    public List<ChargeData> chargeDatas;// 注意顺序代表第几段

    //如果技能选取目标是有范围的,那么这个就是对应的范围限制
    //如果是引导类技能,那么这个就是需要引导的时间

    public float value;

    public override Pipeline_TriggerType GetTriggerType()
    {
        return Pipeline_TriggerType.等待用户输入;
    }
}

[LabelText("可编程节点")]
[LabelWidth(150)]
[Serializable]
public class Pipeline_Programmable : BasePipelineData
{
    //方法1
    //public string abName;//脚本文件所在ab包名
    //public string fileName;//脚本文件名

    //会从对应的ab包中找到对应的file文件,然后找到对应方法,传递参数,执行这个方法
    //暂时只考虑以lua作为脚本文件的编码. 

    //方法2
    //下面这种方式更推荐: 直接C#作为脚本的方式.
    //因为技能实际上很难出现在热更里. 顶多做个平衡性更新.这样的,把数据单独抽离出来就好了.

    [LabelText("脚本")]
    [LabelWidth(100)]
    public IPipeline_PmbNode pmb;


    public override Pipeline_TriggerType GetTriggerType()
    {
        return Pipeline_TriggerType.可编程;
    }
}


public enum FindTargetType
{
    自身,
    我方N人,
    敌方N人,
    我方全体,
    敌方全体,
    自身为中心的范围内,
    距离自己最近的N个队友,
    距离自己最近的N个敌人
}

[LabelText("寻找目标")]
[LabelWidth(150)]
[Serializable]
public class Pipeline_FindTarget : BasePipelineData
{

    public FindTargetType findTargetType;

    // 如果是自身为中心的范围内,那就把这个数/100
    //如果是我方/敌方N人/最近N个队友/最近的N个敌人,那这个就是对应的数量
    public int value;




    public override Pipeline_TriggerType GetTriggerType()
    {
        return Pipeline_TriggerType.自动寻找目标;
    }
}
