using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.HitEffect, typeof(Buff_HitEffect))]

public class BuffEditor_HitEffect: IEditorBuffConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_HitEffect buff = data as Buff_HitEffect;

        buff.hitObjId = EditorGUILayout.DelayedTextField("击中的特效Id", buff.hitObjId, GUILayout.Width(400), GUILayout.Height(20));
        buff.duration = EditorGUILayout.DelayedFloatField("特效存活时间", buff.duration, GUILayout.Width(400), GUILayout.Height(20));
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

