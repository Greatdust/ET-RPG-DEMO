using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.GiveRecover, typeof(Buff_GiveRecover))]
public class BuffEditor_GiveRecover : IEditorBuffConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_GiveRecover buff = data as Buff_GiveRecover;
        EditorGUIUtility.labelWidth = 200;
        buff.hpValue = EditorGUILayout.DelayedFloatField("HP直接回复值", buff.hpValue, GUILayout.Width(400), GUILayout.Height(20));
        buff.hpPct = EditorGUILayout.DelayedFloatField("HP百分比回复值", buff.hpValue, GUILayout.Width(400), GUILayout.Height(20));
        buff.mpValue = EditorGUILayout.DelayedFloatField("MP直接回复值", buff.hpValue, GUILayout.Width(400), GUILayout.Height(20));
        buff.mpPct = EditorGUILayout.DelayedFloatField("MP百分比回复值", buff.hpValue, GUILayout.Width(400), GUILayout.Height(20));
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

