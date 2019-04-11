using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum SkillType
{
    Active,//主动技能
    Passive,//被动技能
}

[Flags]
public enum ActiveSkillTag
{
    DAMAGE,//带伤害
    BUFF,//带Buff
    DEBUFF,
    RESTORE,//带恢复
    INTERRUPT,// 带打断
    CONTROL, // 带控制
    RANGE, // 范围伤害
    LOCK //单体锁定
}

//技能本身是否可以被打断
//霸体类的是属于角色状态,需要另外计算
[Flags]
public enum TypeOfInterruption
{
    UnStoppable = 1, // 不可被打断
    FromDamage, // 受到伤害就打断
    FromNotInControl, // 当玩家不可以控制自己时,被打断
    FromCancel, //被玩家主动取消

    FromAll = FromDamage | FromNotInControl | FromCancel,// 任意可打断的都可以打断
}



[Serializable]
public abstract class BaseSkillData
{
    public enum Pipeline_TriggerType
    {
        碰撞检测,//该项的所有Buff被触发的条件是上一个的所有BUFF触发后,经过几个特殊的,比如碰撞的事件后,触发该项
        固定时间,//一定时间后触发,本身又持续一定的时间
        循环开始,//用以控制后续多少个pipeline节点重复执行多少次.主要目的是实现盖伦E,闪电链,暴风雪这种持续伤害的技能
        等待用户输入,  //这个目的是为了接收用户输入的数据,传输到后续Pipeline中. 比如选择一个目标/方向/范围使用技能,蓄力类技能等,
        自动寻找目标, //这个目的是为了处理一些BUFF直接作用于自身,以自身为中心的范围内的目标,或者敌方队伍全体,我方队伍全体这种情况
        可编程,// 这个目的是暴露全部可以支持的Buff作为借口供外部脚本使用.
        //条件判断 // 用以实现技能执行时满足一些条件执行A,满足另一些条件执行B的情况.比如一个技能的效果是发射一枚火球,但是有30%的概率连续发射两枚,或者周围环境是火焰环境,火球变成火龙
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
            pipelineSignal = "P_" + System.DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString();
        }
    }



    [Serializable]
    public abstract class PipelineDataWithBuff : BasePipelineData
    {
        public List<BuffInSkill> buffs = new List<BuffInSkill>();

    }

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

    [Serializable]
    public class Pipeline_Collision : PipelineDataWithBuff
    {
        [LabelText("输入SIGNAL")]
        [InfoBox("指定哪个BUFF的碰撞检测所检测到的目标作为这个节点下所有BUFF的输入")]
        [LabelWidth(120)]
        public string buffSignal;//指定哪个BUFF的碰撞检测所检测到的目标作为这个节点下所有BUFF的输入
        public override Pipeline_TriggerType GetTriggerType()
        {
            return Pipeline_TriggerType.碰撞检测;
        }
    }

    [Serializable]
    public class Pipeline_CycleStart : BasePipelineData
    {
        [LabelText("执行宽度")]
        [InfoBox("对后续的几个节点进行循环")]
        [LabelWidth(120)]
        public byte cycleRange = 1;//对后续的几个节点进行循环,理论上应该不会超过255,如果出现了,要么游戏太特殊,要么请砍死策划
        [LabelText("执行次数")]
        [LabelWidth(120)]
        public byte repeatCount = 1;//循环执行几次

        //TODO: 如果要考虑盖伦E这种技能(随着英雄等级导致技能的伤害频率提升的),这里还要加几个成长值. 但是DEMO的话无所谓了

        public override Pipeline_TriggerType GetTriggerType()
        {
            return Pipeline_TriggerType.循环开始;
        }
    }

    public enum InputType
    {
        Tar, // 单个目标,锁定类 返回一个目标.
        Dir, // 方向,范围技能 返回一个方向, 
        Pos, // 地点,范围技能, 返回一个地点,
        Charge, // 蓄力类型的技能,返回一个伤害加成和方向

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

    [Serializable]
    public class Pipeline_WaitForInput : BasePipelineData
    {
        public InputType inputType;

        // 如果是蓄力类型的,那么会有一个列表,代表蓄力达到第几段了,每一段增加多少伤害
        public List<ChargeData> chargeDatas;// 注意顺序代表第几段


        public override Pipeline_TriggerType GetTriggerType()
        {
            return Pipeline_TriggerType.等待用户输入;
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

    public interface IActiveConditionData
    {
        string GetBuffActiveConditionType();
    }

    public interface IActiveConditionHandler
    {
        bool MeetCondition(IActiveConditionData data, Unit source);
        void OnRemove(IActiveConditionData data, Unit source);
    }



    [Serializable]
    public class BuffInSkill
    {
        [ReadOnly]
        [LabelText("SIGNAL")]
        [LabelWidth(100)]
        public string buffSignal;//一个Skill中的BUFF和其他BUFF区别开的标志,通常命名为buff_加上buff添加时的序号
        [LabelText("输入SIGNAL")]
        [InfoBox("获取哪个signal的输出作为处理自身BUFF时的输入,可以是Pipeline节点的,可以是其他Buff的")]
        [LabelWidth(100)]
        public string signal_GetInput;//获取哪个signal的输出作为处理自身BUFF时的输入,可以是Pipeline节点的,可以是其他Buff的
        [LabelText("初始启用")]
        [LabelWidth(100)]
        public bool enable = true;//是否处于启用状态
        [LabelText("延迟时间")]
        [LabelWidth(100)]
        public float delayTime;//延迟执行的时间
        [LabelText("BUFF数据")]
        [LabelWidth(100)]
        public IBuffData buffData;//这个是通用的BUFF数据

        public BuffInSkill()
        {
            buffSignal = "B_" + System.DateTime.Now.Millisecond.ToString() + DateTime.Now.ToString();
        }
    }

    [LabelText("技能ID")]
    [LabelWidth(100)]
    public string skillId;//在数据中存储的Id
    [LabelText("技能名")]
    [LabelWidth(100)]
    public string skillName;//显示出来的名字
    [LabelText("资源AB包名")]
    [LabelWidth(100)]
    public string skillAssetsABName;//技能资源所在AB包的前缀名
    [LabelText("冷却时间")]
    [LabelWidth(100)]
    public float costTime;//冷却时间

    public abstract SkillType SkillType { get; }

    [LabelText("使用条件")]
    [LabelWidth(100)]
    public List<IActiveConditionData> activeConditionDatas = new List<IActiveConditionData>();//技能是否可以执行的检测条件
    [LabelText("流程内容")]
    [LabelWidth(100)]
    public LinkedList<BasePipelineData> pipelineDatas = new LinkedList<BasePipelineData>();
    [LabelText("打断类型")]
    [LabelWidth(100)]
    public TypeOfInterruption interruptType = TypeOfInterruption.UnStoppable;// 可以被打断的类型

    [Serializable]
    public struct IncreData
    {
        [LabelText("选定SIGNAL")]
        [LabelWidth(100)]
        public string signal;
        [LabelText("开启")]
        [LabelWidth(100)]
        public bool enable;
    }

    //技能的扩展. 比如暗黑3一个技能可以附加不同的符文,达到不同的效果.
    //或者大量MMORPG中可能出现的一个技能升到几级出现不同的新特性.
    //本身这里不做对技能的更改,只控制对应的pipeline节点,buff的开关状态
    [Serializable]
    public class IncrementUpdate
    {
        [LabelText("扩展名")]
        [LabelWidth(100)]
        public string incrementName;
        [LabelText("扩展数据")]
        [LabelWidth(100)]
        public List<IncreData> incrementData = new List<IncreData>();

    }
    [LabelText("技能扩展")]
    [LabelWidth(100)]
    public List<IncrementUpdate> incrementUpdates;//


}



[Serializable]
public class PassiveSkillData : BaseSkillData
{
    public override SkillType SkillType => SkillType.Passive;
    [LabelText("监听事件")]
    [LabelWidth(100)]
    public bool listenToEvent;//需要持续不断检测作为激活条件的被动技能,具体实现用监听对应事件来做,而不是每帧检测
    [LabelText("事件ID")]
    [LabelWidth(100)]
    [ShowIf("listenToEvent")]
    public string eventIdType;//具体的事件Id
}



[Serializable]
public class ActiveSkillData : BaseSkillData
{
    public override SkillType SkillType
    {
        get
        {
            return SkillType.Active;
        }
    }
    [LabelText("是普通攻击")]
    [LabelWidth(120)]
    public bool isNormalAttack;//是否是普通攻击
    [LabelText("技能标签")]
    [LabelWidth(120)]
    public ActiveSkillTag activeSkillTag;//规定技能的标签.一方面方便玩家了解技能,另一方面可以帮助怪物战斗时选择技能的AI
}

//挂在角色身上的技能附加数据
//角色身上的技能本身只是一个skill的id
[Serializable]
public class BaseSkill_AppendedData
{
    public int level = 1; // 对应技能的等级

    //控制所有signal对应的开启与否的状态
    public Dictionary<string, bool> allSignalState = new Dictionary<string, bool>();
}

 




