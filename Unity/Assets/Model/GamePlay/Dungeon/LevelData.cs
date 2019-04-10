using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public enum VictoryConditionType
    {
        EnemyAllDie,
        ArriveTime
    }

    public enum FailureConditionType
    {
        PlayerAllDie
    }

    public enum LevelFlowTriggerType
    {
        afterTime,
        byCollider,// 通过关卡场景中的碰撞体
        byEvent // 通过关卡中的事件,比如Boss被杀,或者关卡通过之类的
    }

    [Serializable]
    public abstract class BaseLevelFlow
    {
        [LabelText("流程触发条件")]
        public LevelFlowTriggerType triggerType;//流程触发类型

        //下面直接给出3个变量的做法,其实很浪费存储空间,最好的办法应该是根据不同的triggerType分不同的class
        //但这个毕竟是DEMO,无所谓了


        [ShowIf("triggerType",0,true)]
        [LabelText("延迟时间")]
        public float delayTime;
        [ShowIf("triggerType", 1, true)]
        [LabelText("碰撞器的ObjId")]
        public string colliderObjId;//如果场景中的碰撞体触发了事件,那么就是通过EventSystem,找到这里监听的事件并触发了
        [ShowIf("triggerType", 2, true)]
        [LabelText("事件Id")]
        public string eventIdType = string.Empty;
        [HideInInspector]
        [NonSerialized]
        public bool haveTriggerd;//是否已经触发过了
        [LabelText("使用玩家队伍")]
        public bool usePlayerTeam = true;//使用玩家队伍,如果不是,那就是玩家操作其他角色的关卡了,比如大炮,比如情景代入某个NPC
        [ShowIf("usePlayerTeam",false)]
        [LabelText("玩家操控单位的TypeId")]
        public int unitTypeId; // 如果使用的不是玩家队伍,那这里就要配置一下使用哪种unit了

        public abstract void Output();
    }

    //关卡流程中的行为,有可能是出怪,出NPC,出道具,场景交互,出现剧情对话,等.
    //Demo这里只做一个,出怪
    [LabelText("出怪")]
    [Serializable]
    public sealed class LevelFlow_DisplayEnemy : BaseLevelFlow
    {
        [LabelText("出怪的TypeId")]
        public int enemyTypeId;
        [LabelText("数量")]
        public int num = 1;//出多少这种类型的怪

        public override void Output()
        {
            if (haveTriggerd) return;
            Game.EventSystem.Run(EventIdType.DisplayEnemy, enemyTypeId, num);
            haveTriggerd = true;
        }
    }

    [Serializable]
    public class LevelConfigData //每一个关卡胜利/失败条件
    {
        public List<BaseLevelFlow> datas = new List<BaseLevelFlow>();//关卡流程数据
        public IVictoryCondition victoryCondition;//关卡通过条件,继续通往下一个关卡,如果是最后一个关卡,那么胜利后直接进入副本结算
        public IFailureCondition failureCondition;//关卡失败条件,会导致副本提前结束

        public bool CheckVictory(DungeonComponent com)
        {
            if (victoryCondition == null)
            {
                Log.Error("配置有误! 关卡没有胜利条件!");
                return false;
            }
            return victoryCondition.Victory(com);
        }

        public bool CheckFailure(DungeonComponent com)
        {
            if (failureCondition == null)
            {
                return false;
            }
            return failureCondition.Failure(com);
        }
    }

    public interface IVictoryCondition
    {
        bool Victory(DungeonComponent dungeonCom);
    }

    public interface IFailureCondition
    {
        bool Failure(DungeonComponent dungeonCom);
    }


    #region 胜利条件
    /// <summary>
    /// 胜利条件:敌全灭
    /// </summary>
    public class VictoryCondition_EnemyAllDie : IVictoryCondition
    {
        public bool Victory(DungeonComponent dungeonCom)
        {
            foreach (var v in dungeonCom.currDungeonData[dungeonCom.currLevelIndex].datas)
            {
                if (!v.haveTriggerd) return false;
            }
            foreach (var v in dungeonCom.enemyTeam)
            {
                UnitStateComponent unitStateComponent = v.GetComponent<UnitStateComponent>();
                Property_Die property_Die = unitStateComponent.GetCurrState<Property_Die>();
                if (!property_Die.Get())
                    return false;
            }
            return true;
        }
    }

    /// <summary>
    /// 胜利条件:战斗时长达到目标值
    /// </summary>
    public class VictoryCondition_BattleTimingArriveValue : IVictoryCondition
    {
        public long aimTime;
        public bool Victory(DungeonComponent dungeonCom)
        {
            if (dungeonCom.levelTiming >= aimTime) return true;
            return false;
        }
    }
    #endregion
    /// <summary>
    /// 失败条件:我方全灭
    /// 如果加上允许复活的话,那就在判定需要退出副本的流程里做,这里不管
    /// </summary>
    public class FailtureCondition_PlayerAllDie : IFailureCondition
    {
        public bool Failure(DungeonComponent dungeonCom)
        {
            foreach (var v in dungeonCom.playerTeam)
            {
                UnitStateComponent unitStateComponent = v.GetComponent<UnitStateComponent>();
                Property_Die property_Die = unitStateComponent.GetCurrState<Property_Die>();
                if (!property_Die.Get())
                    return false;
            }
            return true;
        }
    }
}
