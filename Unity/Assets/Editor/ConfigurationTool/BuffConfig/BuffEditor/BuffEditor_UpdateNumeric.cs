using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.UpdateNumeric, typeof(Buff_UpdateNumeric))]
public class BuffEditor_UpdateNumeric : IEditorBuffConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_UpdateNumeric buff = data as Buff_UpdateNumeric;
        EditorGUIUtility.labelWidth = 200;

        EditorGUILayout.LabelField("如果是更改对应的数值,请不要选择带Final字眼的数值类型,这类型的数值仅仅用来计算", EditorGUIHelper.GetGUIStyle(Color.blue), GUILayout.Height(22));

        buff.addValueByNumeric = EditorGUILayout.Toggle("改变的值跟施术者身上的属性关联", buff.addValueByNumeric, GUILayout.Width(400), GUILayout.Height(20));

        if (buff.addValueByNumeric)
        {
            buff.sourceNumeric = (NumericType)EditorGUILayout.EnumPopup("关联属性(施术者):", buff.sourceNumeric, GUILayout.Width(400), GUILayout.Height(20));
            buff.coefficient = EditorGUILayout.DelayedFloatField("影响系数: ", buff.coefficient, GUILayout.Width(400), GUILayout.Height(20));
        }

        buff.targetNumeric = (NumericType)EditorGUILayout.EnumPopup("目标属性:", buff.targetNumeric, GUILayout.Width(400), GUILayout.Height(20));
        buff.valueAdd = EditorGUILayout.DelayedFloatField("改变的基础值：", buff.valueAdd, GUILayout.Width(400), GUILayout.Height(20));



    }


}

