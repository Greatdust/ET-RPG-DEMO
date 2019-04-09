using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.DirectDamage, typeof(Buff_DirectDamage))]

public class BuffEditor_DirectDamage : IEditorBuffConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_DirectDamage buff = data as Buff_DirectDamage;

        buff.damageValue = EditorGUILayout.DelayedIntField("伤害值：", buff.damageValue, GUILayout.Width(500));


        buff.damageType = (GameCalNumericTool.DamageType)EditorGUILayout.EnumPopup("伤害类型：", buff.damageType, GUILayout.Width(500));



    }

    public void DrawTrigger(BaseBuffData baseBuffData)
    {
        DrawingBuff(baseBuffData);
    }
}

