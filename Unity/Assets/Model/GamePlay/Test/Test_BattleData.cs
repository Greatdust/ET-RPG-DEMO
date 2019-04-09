using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Test_BattleData : MonoBehaviour
{
    [HideInInspector]
    public int unitTypeId = 1001;
    [HideInInspector]
    public EnterBattleEventData battleEventData =new EnterBattleEventData();
}
