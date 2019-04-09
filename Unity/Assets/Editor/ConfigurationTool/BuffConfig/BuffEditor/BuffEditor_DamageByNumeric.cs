using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.DamageByNumeric, typeof(Buff_DamageByNumeric))]

public class BuffEditor_DamageByNumeric : IEditorBuffConfig,IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {

        Buff_DamageByNumeric buff = data as Buff_DamageByNumeric;

        

        buff.numericType = (NumericType)EditorGUILayout.EnumPopup("影响伤害的数值:", buff.numericType, GUILayout.Width(500));


        buff.coefficient = EditorGUILayout.DelayedFloatField("伤害系数：", buff.coefficient, GUILayout.Width(500));


        buff.damageType = (GameCalNumericTool.DamageType)EditorGUILayout.EnumPopup("伤害类型：", buff.damageType, GUILayout.Width(500));




    }

    public void DrawTrigger(BaseBuffData baseBuffData)
    {
        DrawingBuff(baseBuffData);
    }
}

