using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[EditorBuffActiveCondition(SkillActiveConditionType.CheckBuff, typeof(SkillActiveCondition_CheckBuff))]

public class BuffActiceConditionEditor_CheckBuff : IEditorBuffActiveConditionConfig
{
    public void DrawCondition(BaseSkillData.IActiveConditionData buffActiveConditionData)
    {
        EditorGUIUtility.labelWidth = 200;
        SkillActiveCondition_CheckBuff checkNumeric = buffActiveConditionData as SkillActiveCondition_CheckBuff;
        checkNumeric.buffName = EditorGUILayout.DelayedTextField("检测对应BuffGroup的名字:", checkNumeric.buffName, GUILayout.Width(400), GUILayout.Height(20));
    }

 
}

