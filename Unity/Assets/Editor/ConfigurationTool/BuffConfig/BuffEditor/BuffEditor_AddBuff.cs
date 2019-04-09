using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.AddBuff, typeof(Buff_AddBuff))]

public class BuffEditor_AddBuff : IEditorBuffConfig,IEditorBuff_TriggerConfig,IEditorBuff_TimelineConfig
{
    public string[] stringArray;
    public void DrawingBuff(BaseBuffData data)
    {
        if (stringArray == null)
        {
          
            List<string> strs = new List<string>();
            strs.AddRange(EditorBuffConfigHelper.BuffConfigDic.Keys);
            strs.Remove(BuffIdType.AddBuff);
            stringArray = strs.ToArray();


        }
        EditorGUILayout.Space();
        EditorGUILayout.Space();
        EditorGUILayout.Space();

        Buff_AddBuff buff = data as Buff_AddBuff;
        var selectedSkill = SkillConfigEditor.instance.selectedSkill;
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("技能使用后给目标添加的BUFF组:", GUILayout.Height(22));
            EditorGUILayout.LabelField("这些BUFF会组成一个Buff组,添加时,一起作用于目标,持续时间到时,一起消失", GUILayout.Height(22));
            EditorBuffGroupHelper.DrawBuffGroup(buff.buffGroup, stringArray, true);
        }
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


