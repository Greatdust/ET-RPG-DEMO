using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public enum UnitTeam
{
    Player,
    Enemy
}
public class UnitActionData
{
    public Unit mUnit;
    public UnitTeam mTeam;
    public float currInterval;//行动条时长
    public float currTiming;//当前计时
    public Vector3 startPos;
    public Vector3 startAngle;
    public bool die;
}

public interface IVictoryCondition
{
    bool Victory(IBattleMgr battleMgr);
}

public interface IFailureCondition
{
    bool Failure(IBattleMgr battleMgr);
}

public class BattleData
{
    public List<UnitActionData> playerTeam =new List<UnitActionData>();
    public List<UnitActionData> enemyTeam =new List<UnitActionData>();
    public IVictoryCondition victoryCondition;
    public IFailureCondition failureCondition;
}

public enum VictoryConditionType
{
    EnemyAllDie,
    ArriveTime
}

public enum FailureConditionType
{
    PlayerAllDie
}

public interface IBattleMgr
{
    BattleData BattleData { get; set; }
    float BattleTiming { get; set; }
    bool Waiting { get; set; }
    void Load(BattleData battleData);
    void DispatchUnitAction(UnitActionData actionData);
}

#region 胜利条件
/// <summary>
/// 胜利条件:敌全灭
/// </summary>
public class VictoryCondition_EnemyAllDie : IVictoryCondition
{
    public bool Victory(IBattleMgr battleMgr)
    {
        foreach (var v in battleMgr.BattleData.enemyTeam)
        {
            if (!v.die)
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
    public float aimTime;
    public bool Victory(IBattleMgr battleMgr)
    {
        if (battleMgr.BattleTiming >= aimTime)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public VictoryCondition_BattleTimingArriveValue(float aimTime)
    {
        this.aimTime = aimTime;
    }
}
#endregion
/// <summary>
/// 失败条件:我方全灭
/// </summary>
public class FailtureCondition_PlayerAllDie : IFailureCondition
{
    public bool Failure(IBattleMgr battleMgr)
    {
        foreach (var v in battleMgr.BattleData.playerTeam)
        {
            if (!v.die)
                return false;
        }
        return true;
    }
}

