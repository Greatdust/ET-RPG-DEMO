using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.EnhanceSkillEffect, typeof(Buff_EnhanceSkillEffect))]
[BuffNonTarget]
public class BuffEditor_EnhanceSkillEffect : IEditorBuffConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_EnhanceSkillEffect buff = data as Buff_EnhanceSkillEffect;
        EditorGUIUtility.labelWidth = 200;
        buff.skillId = EditorGUILayout.DelayedTextField("提升的技能Id", buff.skillId, GUILayout.Width(400), GUILayout.Height(20));
        buff.effectData.coefficientAddPct = EditorGUILayout.DelayedFloatField("提升技能效果百分比", buff.effectData.coefficientAddPct, GUILayout.Width(400), GUILayout.Height(20));
        buff.effectData.critical = EditorGUILayout.Toggle("设置效果必定暴击:", buff.effectData.critical, GUILayout.Width(400), GUILayout.Height(20));
    }

    public void DrawTimeLineBuff(BaseBuffData timeLineBuff)
    {
        DrawingBuff(timeLineBuff);
    }

    public void DrawTrigger(BaseBuffData baseBuffData)
    {
        DrawingBuff(baseBuffData);
    }
}

