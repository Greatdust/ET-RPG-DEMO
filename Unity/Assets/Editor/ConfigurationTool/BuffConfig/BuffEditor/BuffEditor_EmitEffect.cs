using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.EmitEffectInSkill, typeof(Buff_EmitEffect))]
[BuffHaveCollisionEvent]
public class BuffEditor_EmitEffect: IEditorBuffConfig,IEditorBuff_TimelineConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_EmitEffect buff = data as Buff_EmitEffect;

        buff.emitObjId = EditorGUILayout.DelayedTextField("发射的特效Id", buff.emitObjId, GUILayout.Width(400), GUILayout.Height(20));

        bool b = EditorStateHelper.GetFold(buff.effectParams.GetHashCode());
        b = EditorGUILayout.Foldout(b, "特效参数配置");
        EditorStateHelper.SetFold(buff.effectParams.GetHashCode(), b);
        if (b)
        {
            ReorderableList reorderableList = EditorStateHelper.GetReorderableList(buff.effectParams.GetHashCode(), buff.effectParams, typeof(string),
                (rect, index, active, foucs) => { buff.effectParams[index] = EditorGUI.TextField(rect, "参数" + index.ToString(), buff.effectParams[index]); },
                addlist =>
                {
                    buff.effectParams.Add(string.Empty);
                },
                addlist =>
                {
                    buff.effectParams.RemoveAt(addlist.index);
                }
                );

            reorderableList.DoLayoutList();
        }


        buff.lockTarget = EditorGUILayout.Toggle("锁定目标", buff.lockTarget, GUILayout.Width(400), GUILayout.Height(20));
        buff.emitSpeed = EditorGUILayout.FloatField("发射的特效飞行速度", buff.emitSpeed, GUILayout.Width(400), GUILayout.Height(20));

        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {

            EditorGUILayout.LabelField("x代表前方偏移量,y代表右方偏移量,z代表上方偏移量", GUILayout.Height(20));
            buff.emitStartPos = EditorGUILayout.Vector3Field("  发射的特效起始位置偏差", buff.emitStartPos, GUILayout.Width(400), GUILayout.Height(20));
            EditorGUILayout.Space();
            EditorGUILayout.Space();
            if (buff.lockTarget)
            {
                buff.reverseDir = EditorGUILayout.ToggleLeft("特效从目标飞向使用者", buff.reverseDir, GUILayout.Width(400), GUILayout.Height(20));
            }
            else
            {
                buff.emitDir = EditorGUILayout.Vector3Field("  发射的特效初始方向偏差", buff.emitDir, GUILayout.Width(400), GUILayout.Height(20));
                EditorGUILayout.Space();
                EditorGUILayout.Space();
            }
            buff.duration = EditorGUILayout.FloatField("特效生命周期", buff.duration, GUILayout.Width(450), GUILayout.Height(20));
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

