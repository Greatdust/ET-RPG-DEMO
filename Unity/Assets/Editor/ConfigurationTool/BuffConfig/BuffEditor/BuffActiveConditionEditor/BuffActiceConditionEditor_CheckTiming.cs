using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[EditorBuffActiveCondition(SkillActiveConditionType.CheckTiming, typeof(SkillActiveCondition_CheckTiming))]

public class BuffActiceConditionEditor_CheckTiming : IEditorBuffActiveConditionConfig
{
    public void DrawCondition(BaseSkillData.IActiveConditionData buffActiveConditionData)
    {
        EditorGUIUtility.labelWidth = 200;
        SkillActiveCondition_CheckTiming checkNumeric = buffActiveConditionData as SkillActiveCondition_CheckTiming;
        checkNumeric.timeSpan = EditorGUILayout.DelayedFloatField("每多少秒检测一次", checkNumeric.timeSpan, GUILayout.Width(400), GUILayout.Height(20));
    }

 
}

