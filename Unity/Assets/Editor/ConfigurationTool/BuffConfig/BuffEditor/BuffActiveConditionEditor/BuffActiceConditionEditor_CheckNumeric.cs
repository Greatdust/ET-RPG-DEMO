using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[EditorBuffActiveCondition(SkillActiveConditionType.CheckNumeric, typeof(SkillActiveCondition_CheckNumeric))]

public class BuffActiceConditionEditor_CheckNumeric : IEditorBuffActiveConditionConfig
{
    public void DrawCondition(BaseSkillData.IActiveConditionData buffActiveConditionData)
    {
        EditorGUIUtility.labelWidth = 200;
        SkillActiveCondition_CheckNumeric checkNumeric = buffActiveConditionData as SkillActiveCondition_CheckNumeric;
        checkNumeric.numericType = (NumericType)EditorGUILayout.EnumPopup("检测的数值", checkNumeric.numericType, GUILayout.Width(400), GUILayout.Height(20));
        checkNumeric.realtionType = (NumericRealtionType)EditorGUILayout.EnumPopup("关系", checkNumeric.realtionType, GUILayout.Width(400), GUILayout.Height(20));
        checkNumeric.aimValue = EditorGUILayout.DelayedFloatField("目标值", checkNumeric.aimValue, GUILayout.Width(400), GUILayout.Height(20));
    }

 
}

