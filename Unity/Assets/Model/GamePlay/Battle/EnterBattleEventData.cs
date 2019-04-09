using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


[Serializable]
public class EnterBattleEventData
{
    [Serializable]
    public struct EnemyConfig
    {
        public int unitTypeId;
        public int minNum;
        public int maxNum;
    }

    public bool usePlayerTeam = true;//使用玩家方角色,为假则完全使用我方NPC列表里的角色

    public string sceneName;

    public List<int> unitInOurTeam;//我方NPC列表,这里的int是unitTypeId
    public List<EnemyConfig> unitInEnemyTeam;//敌方NPC列表
    public VictoryConditionType victoryConditionType= VictoryConditionType.EnemyAllDie;

    public float aimTimingInBattle;//如果胜利条件是战斗时长达到一定程度,这就是目标值

    public FailureConditionType failureConditionType = FailureConditionType.PlayerAllDie;

}



