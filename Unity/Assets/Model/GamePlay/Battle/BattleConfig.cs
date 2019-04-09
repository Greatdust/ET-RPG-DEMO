using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName ="GameConfig/BattleConfig")]
public class BattleConfig : ScriptableObject
{
    [LabelText("角色行动的基础间隔时间")]
    [SerializeField]
    private float baseInterval = 2.5f;

    [LabelText("玩家队伍成员最大数量")]
    [SerializeField]
    private int unitMaxNumInPlayerTeam = 3;

    [LabelText("敌人队伍成员最大数量")]
    [SerializeField]
    private int unitMaxNumInEnemyTeam = 6;

    public float BaseInterval { get => baseInterval; }
    public int UnitMaxNumInPlayerTeam { get => unitMaxNumInPlayerTeam; }
    public int UnitMaxNumInEnemyTeam { get => unitMaxNumInEnemyTeam; }
}
