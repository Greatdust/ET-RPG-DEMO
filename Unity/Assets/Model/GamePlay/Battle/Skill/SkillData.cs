using ETModel;
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

public enum ActiveSkillTag
{
    单体伤害类,
    群体伤害类,
    单体BUFF类,
    群体BUFF类,
    单体治疗类,
    群体治疗类,
}




[Serializable]
public abstract class BaseSkillData
{
    public enum Pipeline_TriggerType
    {
        碰撞检测,//该项的所有Buff被触发的条件是上一个的所有BUFF触发后,经过几个特殊的,比如碰撞的事件后,触发该项
        固定时间,//一定时间后自动触发
        循环开始,//用以控制后续多少个pipeline节点重复执行多少次.主要目的是实现盖伦E,闪电链,暴风雪这种持续伤害的技能
        技能结束,//回合制游戏独有的,用以判定角色技能行动结束
        //接收用户输入,  //这个目的是为了接收用户输入的数据,传输到下一个Pipeline中.但是回合制游戏不需要这东西,所以注释掉
        //可编程节点数据,// 这个目的是暴露全部可以支持的Buff作为借口供外部脚本使用,目前的回合制游戏不需要这东西,所以注释掉
        //条件判断 // 用以实现技能执行时满足一些条件执行A,满足另一些条件执行B的情况.比如一个技能的效果是发射一枚火球,但是有30%的概率连续发射两枚,或者周围环境是火焰环境,火球变成火龙
    }
    [Serializable]
    public abstract class BasePipelineData
    {
        public abstract Pipeline_TriggerType GetTriggerType();
        public bool enable = true;//是否处于启用状态
    }



    [Serializable]
    public abstract class PipelineDataWithBuff : BasePipelineData
    {
        public List<BuffInSkill> buffs = new List<BuffInSkill>();

    }

    [Serializable]
    public class Pipeline_FixedTime : PipelineDataWithBuff
    {
        public float delayTime;//等待多少秒后执行
        public float fixedTime;//该阶段一共持续多少秒

        public override Pipeline_TriggerType GetTriggerType()
        {
            return Pipeline_TriggerType.固定时间;
        }
    }

    [Serializable]
    public class Pipeline_Collision : PipelineDataWithBuff
    {
        public string buffSignal;//指定哪个BUFF的碰撞检测所检测到的目标作为这个节点下所有BUFF的输入
        public override Pipeline_TriggerType GetTriggerType()
        {
            return Pipeline_TriggerType.碰撞检测;
        }
    }

    [Serializable]
    public class Pipeline_CycleStart : BasePipelineData
    {
        public byte cycleRange = 1;//对后续的几个节点进行循环,理论上应该不会超过255
        public byte repeatCount = 1;//循环执行几次

        public override Pipeline_TriggerType GetTriggerType()
        {
            return Pipeline_TriggerType.循环开始;
        }
    }


    [Serializable]
    public class Pipeline_SkillEnd : BasePipelineData
    {
        public float duration;//收尾阶段技能持续时长

        public override Pipeline_TriggerType GetTriggerType()
        {
            return Pipeline_TriggerType.技能结束;
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
        public string buffSignal;//一个Skill中的BUFF和其他BUFF区别开的标志,通常命名为buff_加上buff添加时的序号
        public string buffSignal_GetInput;//获取哪个buff的输出作为处理自身BUFF时的输入
        public bool enable = true;//是否处于启用状态
        public float delayTime;//延迟执行的时间
        public BaseBuffData buffData;//这个是通用的BUFF数据
        /// <summary>
        /// BUFF的目标是独立的,在技能里,如果一个子BUFF的目标和技能目标一样,那么直接拿到技能的目标作为自己的目标
        /// 如果在装备或者道具里,那么就是传递一个使用的对象,作为自己的目标.
        /// </summary>
        public BuffTargetType TargetType;
        [NonSerialized]
        public ActiveSkillData ParentSkillData;
    }

    public string skillId;//在数据中存储的Id
    public string skillName;//显示出来的名字
    public string skillAssetsABName;//技能资源所在AB包的前缀名

    public float skillExcuteSpeed = 1;//技能执行速度,用以控制技能中动画,特效的播放速度.

    public abstract SkillType SkillType { get; }
    public List<IActiveConditionData> activeConditionDatas = new List<IActiveConditionData>();//技能是否可以执行的检测条件
    public LinkedList<BasePipelineData> pipelineDatas = new LinkedList<BasePipelineData>();
    [NonSerialized]
    public Dictionary<string, List<IBuffReturnedValue>> buffReturnValues = new Dictionary<string, List<IBuffReturnedValue>>();//存储技能执行过程中产生的中间数据
    [NonSerialized]
    public Dictionary<BaseBuffData, Action> collisionEvents = new Dictionary<BaseBuffData, Action>();//存储技能执行过程中产生的碰撞事件,string是对应的BuffSignal
    public Dictionary<string, BuffInSkill> AllBuffInSkill = new Dictionary<string, BuffInSkill>();//包含了位于timeline上面的所有BaseBUFF和TriggerBuff\
    public BuffTargetType mainTargetType;//技能的主要目标类型

    public const string SkillMainTargetSignal = "SkillMainTarget";//这个是技能的主目标列表的BUFFSignal
    
    [NonSerialized]
    public Unit SourceUnit; //BUFF来源方


    public int buffSingalIndex;//用来给BuffSignal计数用的

    public void AddPineLine(BasePipelineData pipelineData, int insertIndex)
    {
        if (insertIndex >= pipelineDatas.Count)
        {

            pipelineDatas.AddLast(pipelineData);
            return;
        }
        int num = 0;
        LinkedListNode<BasePipelineData> linkedListNode = pipelineDatas.First;
        while (num < insertIndex)
        {
            linkedListNode = linkedListNode.Next;
            num++;
        }
        pipelineDatas.AddBefore(linkedListNode, new LinkedListNode<BasePipelineData>(pipelineData));
    }

    public void AddBuff(PipelineDataWithBuff skillPinelineData, BuffInSkill buff)
    {
        buff.buffSignal = "TL_buff_" + buffSingalIndex.ToString();
        buffSingalIndex++;
        skillPinelineData.buffs.Add(buff);
        AllBuffInSkill.Add(buff.buffSignal, buff);
    }

    public void AddReturnValue(string buffSignal, IBuffReturnedValue buffReturnedValue)
    {
        List<IBuffReturnedValue> list = null;
        if (!buffReturnValues.TryGetValue(buffSignal, out list))
        {
            list = new List<IBuffReturnedValue>();
            buffReturnValues[buffSignal] = list;
        }
        list.Add(buffReturnedValue);
    }

    public void RemoveBuff(PipelineDataWithBuff skillPinelineData, BuffInSkill buff)
    {
        skillPinelineData.buffs.Remove(buff);
    }

    public void RemoveBaseBuffInBuffDic(BuffInSkill buff)
    {
        AllBuffInSkill.Remove(buff.buffSignal);
    }

    public void RemovePipeLineData(BasePipelineData basePipelineData)
    {
        var node = pipelineDatas.Find(basePipelineData);

        PipelineDataWithBuff pipelineDataWithBuff = node.Value as PipelineDataWithBuff;
        if (pipelineDataWithBuff!=null && pipelineDataWithBuff.buffs.Count > 0)
            foreach (var v in pipelineDataWithBuff.buffs)
            {
                AllBuffInSkill.Remove(v.buffSignal);
            }

        var next = node.Next;
        pipelineDatas.Remove(node);
        node = next;

    }

    public void AddBaseBuffInDic(BuffInSkill buff)
    {
        if (string.IsNullOrEmpty(buff.buffSignal))
        {
            buff.buffSignal = "IN_buff_" + buffSingalIndex.ToString();
            buffSingalIndex++;
        }
        AllBuffInSkill.Add(buff.buffSignal, buff);
    }

    [Serializable]
    public class IncrementUpdate
    {
        public string incrementName;
        public List<Data> incrementData = new List<Data>();

        [Serializable]
        public class Data
        {
            public string buffSignal;
            public bool enable;
        }
    }

    public List<IncrementUpdate> incrementUpdates;//

    public void AddIncrementBuff(IncrementUpdate update)
    {
        if (incrementUpdates == null)
            incrementUpdates = new List<IncrementUpdate>();
        incrementUpdates.Add(update);
    }


}

[Serializable]
public class PassiveSkillData : BaseSkillData
{
    public override SkillType SkillType => SkillType.Passive;
    public bool listenToEvent;//需要持续不断检测作为激活条件的被动技能,具体实现用监听对应事件来做,而不是每帧检测
    public string eventIdType;//具体的事件Id
    [NonSerialized]
    public bool apply;
    [NonSerialized]
    public AEvent<long> aEvent;
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
    public bool isNormalAttack;//是否是普通攻击
    public ActiveSkillTag activeSkillTag;//规定技能的标签,自动战斗时选择技能类型用,暂时只支持单独标签
    public float timeImpact;//技能使用后，下一次读条增加多少时间
}

//执行BUFF时,获得的返回值,用以接下来执行BUFF时的输入
public interface IBuffReturnedValue
{

}

//移动相关BUFF需要的输入
public struct BuffReturnedValue_MoveData: IBuffReturnedValue
{
    public Vector3 startPos;
    public Vector3 endPos;
    public Quaternion startDir;
}

public struct BuffReturnedValue_TargetUnit : IBuffReturnedValue
{
    public Unit target;
    public float playSpeedScale;//播放速度的缩放系数,用以控制动作,音频,特效等的播放速度
}




