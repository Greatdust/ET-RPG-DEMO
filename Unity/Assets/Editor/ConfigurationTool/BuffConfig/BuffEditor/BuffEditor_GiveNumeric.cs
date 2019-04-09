using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.GiveNumeric, typeof(Buff_GiveNumeric))]
[BuffNonTarget]
public class BuffEditor_GiveNumeric : IEditorBuffConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_GiveNumeric buff = data as Buff_GiveNumeric;
        EditorGUIUtility.labelWidth = 200;
        buff.numericType = (NumericType)EditorGUILayout.EnumPopup("影响角色的数值类型: ", buff.numericType, GUILayout.Width(400), GUILayout.Height(20));
        buff.value = EditorGUILayout.DelayedFloatField("影响值", buff.value, GUILayout.Width(400), GUILayout.Height(20));
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

