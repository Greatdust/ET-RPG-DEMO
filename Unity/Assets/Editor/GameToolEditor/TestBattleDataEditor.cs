using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(Test_BattleData))]
public class TestBattleDataEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        Test_BattleData test_BattleData = target as Test_BattleData;
        test_BattleData.battleEventData.usePlayerTeam = false;
        EditorEnterBattleHelper.BattleConfig(test_BattleData.battleEventData);
        if (GUI.changed)
        {
            EditorUtility.SetDirty(test_BattleData);
        }
    }
}
