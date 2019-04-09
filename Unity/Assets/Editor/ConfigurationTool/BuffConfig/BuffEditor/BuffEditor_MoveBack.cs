using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.MoveBack, typeof(Buff_MoveBack))]

public class BuffEditor_MoveBack : IEditorBuffConfig,IEditorBuff_TimelineConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_MoveBack buff = data as Buff_MoveBack;

        buff.flash = EditorGUILayout.Toggle("是否为瞬间移动", buff.flash);
        if (!buff.flash)
        {
            buff.moveDuration = EditorGUILayout.FloatField("移动时间", buff.moveDuration, GUILayout.Width(400), GUILayout.Height(23));
        }
        buff.resetDir = EditorGUILayout.Toggle("调整方向:", buff.resetDir, GUILayout.Width(400), GUILayout.Height(20));


    }

    public void DrawTimeLineBuff(BaseBuffData timeLineBuff)
    {
        DrawingBuff(timeLineBuff);
    }
}

