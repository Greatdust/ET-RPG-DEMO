using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
[ObjectSystem]
public class BattleMgrAwakeSystem : AwakeSystem<BattleMgrComponent>
{
    public override void Awake(BattleMgrComponent self)
    {
        self.Awake();
    }
}
[ObjectSystem]
public class BattleMgrUpdateSystem : UpdateSystem<BattleMgrComponent>
{
    public override void Update(BattleMgrComponent self)
    {
        self.Update();
    }
}



public class BattleMgrComponent : ETModel.Component, IBattleMgr
{
    public bool battleStart;//战斗是否开始
    public BattleConfig mConfig;//读取到的战斗配置
    public bool AutoCombat { get; set; }//玩家是否开启了自动战斗

    public UnitActionData currUnitInAction;//记录一下当前正在处于行动中的UnitActionData,免得查找
    public float BattleTiming { get ; set ; }//战斗时长计时
    public bool Waiting { get ; set ; }//等待当前的UnitAction结束
    public BattleData BattleData { get ; set; } //战斗中需要用到的数据
    public static BattleMgrComponent Instance { get; private set; }

    public Dictionary<long, UnitActionData> unitActionDataDic;

    private float restoreTimeSpan = 1;//自动回复效果间隔
    private float restoreTiming;

    

   

    #region private
    void InitBattleData(UnitActionData unitActionData, Transform trans)
    {
        unitActionData.currTiming = 0;
        unitActionData.mUnit.Position = trans.position;
        unitActionData.startPos = trans.position;
        unitActionData.mUnit.GameObject.transform.eulerAngles = trans.eulerAngles;
        unitActionData.startAngle = trans.eulerAngles;
    }


    public void OnBattleEnd()
    {

    }

    bool CheckUnitAction()
    {
        foreach (var v in BattleData.playerTeam)
        {
            if (v.die) continue;
            if (v.currTiming >= v.currInterval)
            {
                DispatchUnitAction(v);
                return true;
            }
        }
        foreach (var v in BattleData.enemyTeam)
        {
            if (v.die) continue;
            if (v.currTiming >= v.currInterval)
            {
                DispatchUnitAction(v);
                return true;
            }
        }
        return false;
    }

    //void UpdateUnitAction()
    //{
    //    currUnitInAction = null;
    //    BattleTiming += Time.deltaTime;

    //    foreach (var v in BattleData.playerTeam)
    //    {
    //        if (v.die) continue;
    //        v.currTiming += Time.deltaTime;
    //        Game.EventSystem.Run(EventIdType.SendUnitActionRate, v.mUnit.Id, v.currTiming / v.currInterval);
    //    }
    //    foreach (var v in BattleData.enemyTeam)
    //    {
    //        if (v.die) continue;
    //        v.currTiming += Time.deltaTime;
    //        Game.EventSystem.Run(EventIdType.SendUnitActionRate, v.mUnit.Id, v.currTiming / v.currInterval);
    //    }
    //}


    #endregion

    #region UnityEvent
    public void Update()
    {
        if (!battleStart) return;
        //在某个角色的行动期间,所有角色的行动计时暂停
        if (Waiting) return;
        //计算自动回复效果
        if (BattleTiming - restoreTiming >= restoreTimeSpan)
        {
            restoreTiming = BattleTiming;
            foreach (var v in unitActionDataDic)
            {
                GameCalNumericTool.CalRestore(v.Value.mUnit);
            }
        }
        //先判断是否满足胜利条件
        if (BattleData.victoryCondition.Victory(this))
        {
            //TODO: Dispatch Victory Event
            Debug.Log("战斗胜利!");
           // BattleReward battleReward = BattleEventHandler.HandleReward(true);
            //Game.EventSystem.Run(EventIdType.BattleVictory, battleReward);
            battleStart = false;
            Waiting = false;
            return;
        }
        //后判断是否满足失败条件
        if (BattleData.failureCondition.Failure(this))
        {
            //TODO:Dispatch Failture Event
            Debug.Log("战斗失败!");
            //BattleReward battleReward = BattleEventHandler.HandleReward(false);
            //Game.EventSystem.Run(EventIdType.BattleFailture, battleReward);
            battleStart = false;
            Waiting = false;
            return;
        }
        //检测轮到哪个单位开始行动了
        if (CheckUnitAction())
        {

            return;
        }
        //更新每个单位的行动吟唱时间,以及整场战斗的用时(不考虑使用技能花费的时间)
        //UpdateUnitAction();
    }
    #endregion
    #region public

    public void DispatchUnitAction(UnitActionData actionData)
    {
        //TODO: Dispatch Unit Action
        Waiting = true;
        currUnitInAction = actionData;

    }

    public void Awake()
    {
        Instance = this;
       // mConfig = ResourcesHelper.Load<BattleConfig>("BattleConfig");
        unitActionDataDic = new Dictionary<long, UnitActionData>();

    }

    public void Load(BattleData battleData)
    {
        AutoCombat = true;
        BattleTiming = 0;
        restoreTiming = 0;
        unitActionDataDic.Clear();
        this.BattleData = battleData;
        this.BattleData.enemyTeam.ForEach(InitActionData);
        this.BattleData.playerTeam.ForEach(InitActionData);


        battleStart = true;
    }

    public void InitActionData(UnitActionData actionData)
    {
       // InitActionData(actionData, 0);
    }

    public void SetUnitDie(long unitid)
    {
        UnitActionData unitActionData = unitActionDataDic[unitid];
        unitActionData.die = true;
        //GameObject.Destroy(unitActionData.mUnit.GameObject);
        //if (unitActionData.mTeam == UnitTeam.Enemy)
        //{
        //    if (BattleMgrComponent.Instance.BattleData.enemyTeam.Contains(unitActionData))
        //    {
        //        BattleMgrComponent.Instance.BattleData.enemyTeam.Remove(unitActionData);
        //    }
        //}
        //else
        //{
        //    if (BattleMgrComponent.Instance.BattleData.playerTeam.Contains(unitActionData))
        //    {
        //        BattleMgrComponent.Instance.BattleData.playerTeam.Remove(unitActionData);
        //    }
        //}
        //unitActionDataDic.Remove(unitid);
    }

    #endregion








}
