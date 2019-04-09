using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.Move, typeof(Buff_Move))]

public class BuffEditor_Move: IEditorBuffConfig,IEditorBuff_TimelineConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_Move buff = data as Buff_Move;

        buff.flash = EditorGUILayout.Toggle("是否为瞬间移动", buff.flash);
        if (!buff.flash)
        {
            buff.moveDuration = EditorGUILayout.FloatField("移动时间", buff.moveDuration, GUILayout.Width(400), GUILayout.Height(23));
        }
        EditorGUIHelper.HorizontalLayout("目标点的偏差距离,方向为目标到技能使用者", 300, 20, () =>
           {
               buff.targetPosOffset = EditorGUILayout.FloatField(buff.targetPosOffset, GUILayout.Width(400), GUILayout.Height(23));
           });
        buff.resetDir = EditorGUILayout.Toggle("调整方向:", buff.resetDir, GUILayout.Width(400), GUILayout.Height(20));
        EditorGUIHelper.HorizontalLayout("如果移动自带动作,那么对应动作的bool值变量名:", 300, 20, () =>
        {
            buff.animatorBoolValue = EditorGUILayout.DelayedTextField(buff.animatorBoolValue, GUILayout.Width(400), GUILayout.Height(23));
        });
    }

    public void DrawTimeLineBuff(BaseBuffData timeLineBuff)
    {
        DrawingBuff(timeLineBuff);
    }
}

