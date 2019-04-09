using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[EditorBuffActiveCondition(SkillActiveConditionType.CheckHPMP, typeof(SkillActiveCondition_CheckHPMP))]

public class BuffActiceConditionEditor_CheckHPMP : IEditorBuffActiveConditionConfig
{
    public void DrawCondition(BaseSkillData.IActiveConditionData buffActiveConditionData)
    {
        EditorGUIUtility.labelWidth = 200;
        SkillActiveCondition_CheckHPMP checkHPMP = buffActiveConditionData as SkillActiveCondition_CheckHPMP;
        checkHPMP.costHp = EditorGUILayout.DelayedFloatField("消耗血量", checkHPMP.costHp, GUILayout.Width(400), GUILayout.Height(20));
        checkHPMP.costMp = EditorGUILayout.DelayedFloatField("消耗法力", checkHPMP.costMp, GUILayout.Width(400), GUILayout.Height(20));
        checkHPMP.costHpInPct = EditorGUILayout.DelayedFloatField("消耗血量(百分比)", checkHPMP.costHpInPct, GUILayout.Width(400), GUILayout.Height(20));
        checkHPMP.costMpInPct = EditorGUILayout.DelayedFloatField("消耗法力(百分比)", checkHPMP.costMpInPct, GUILayout.Width(400), GUILayout.Height(20));
    }

 
}

