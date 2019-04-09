using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.CostHPMP, typeof(Buff_CostHP_MP))]

public class BuffEditor_CostHPMP : IEditorBuffConfig,IEditorBuff_TimelineConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_CostHP_MP buff = data as Buff_CostHP_MP;

        buff.costHp = EditorGUILayout.DelayedFloatField("消耗血量", buff.costHp, GUILayout.Width(400), GUILayout.Height(20));
        buff.costMp = EditorGUILayout.DelayedFloatField("消耗法力", buff.costMp, GUILayout.Width(400), GUILayout.Height(20));
        buff.costHpInPct = EditorGUILayout.DelayedFloatField("消耗血量(百分比)", buff.costHpInPct, GUILayout.Width(400), GUILayout.Height(20));
        buff.costMpInPct = EditorGUILayout.DelayedFloatField("消耗法力(百分比)", buff.costMpInPct, GUILayout.Width(400), GUILayout.Height(20));
    }

    public void DrawTimeLineBuff(BaseBuffData timeLineBuff)
    {
        DrawingBuff(timeLineBuff);
    }
}

