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
    DAMAGE = 1<<1,//带伤害
    BUFF = 1<<2,//带Buff
    DEBUFF = 1<<3,
    RESTORE = 1<<4,//带恢复
    INTERRUPT = 1<<5,// 带打断
    CONTROL = 1<<6, // 带控制
    RANGE = 1<<7, // 范围伤害
    LOCK = 1<<8 //单体锁定
}

//技能本身是否可以被打断
//霸体类的是属于角色状态,需要另外计算

public enum TypeOfInterruption
{
    UnStoppable = 1, // 不可被打断
    FromDamage, // 受到伤害就打断
    FromNotInControl, // 当玩家不可以控制自己时,被打断
    FromCancel, //被玩家主动取消

    FromAll ,// 任意可打断的都可以打断
}



[Serializable]
public abstract class BaseSkillData
{
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
        [InfoBox("获取哪些signal的输出作为处理自身BUFF时的输入,可以是Pipeline节点的,可以是其他Buff的")]
        [LabelWidth(100)]
        public string[] signals_GetInput;//获取哪些signal的输出作为处理自身BUFF时的输入,可以是Pipeline节点的,可以是其他Buff的
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
            //需要保证每个技能的每个buffSignal 都是唯一的
            buffSignal = "B_" + IdGenerater.GenerateId().ToString();
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
    public float coolDown;//冷却时间

    public abstract SkillType SkillType { get; }

    [LabelText("使用条件")]
    [LabelWidth(100)]
    public List<IActiveConditionData> activeConditionDatas = new List<IActiveConditionData>();//技能是否可以执行的检测条件
    [LabelText("流程内容,排列顺序等于执行顺序")]
    [LabelWidth(100)]
    public List<BasePipelineData> pipelineDatas = new List<BasePipelineData>();
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


    public List<IBuffData> GetAllBuffs(params string[] types)
    {
        List<IBuffData> buffDatas = new List<IBuffData>();

        foreach (var v in pipelineDatas)
        {
            PipelineDataWithBuff withBuff = v as PipelineDataWithBuff;
            if (withBuff != null)
            {
                foreach (var buff in withBuff.buffs)
                {
                    foreach (var type in types)
                    {
                        if (buff.buffData.GetBuffIdType() == type)
                        {
                            buffDatas.Add(buff.buffData);
                        }
                    }
                }
            }
        }

        return buffDatas;
    }

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

 




