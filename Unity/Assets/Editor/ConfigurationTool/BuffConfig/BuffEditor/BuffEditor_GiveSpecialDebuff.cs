using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.GiveSpecialDebuff, typeof(Buff_GiveSpecialDebuff))]
public class BuffEditor_GiveSpecialDebuff : IEditorBuffConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_GiveSpecialDebuff buff = data as Buff_GiveSpecialDebuff;
        EditorGUIUtility.labelWidth = 200;
        buff.aimStackNum = EditorGUILayout.DelayedIntField("叠加多少层之后才施加效果: ", buff.aimStackNum, GUILayout.Width(400), GUILayout.Height(20));
        buff.restrictionType = (RestrictionType)EditorGUILayout.EnumPopup("施加的效果类型:", buff.restrictionType, GUILayout.Width(400), GUILayout.Height(20));
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

