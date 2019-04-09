using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.PlayAnim, typeof(Buff_PlayAnim))]
[BuffNonTarget]
public class BuffEditor_PlayAnim: IEditorBuffConfig,IEditorBuff_TimelineConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_PlayAnim buff = data as Buff_PlayAnim;
        using (new EditorGUILayout.HorizontalScope())
        {
            buff.playTime = EditorGUILayout.DelayedFloatField("动画播放时间", buff.playTime, GUILayout.Width(400), GUILayout.Height(20));
            EditorGUILayout.LabelField("0代表默认播放时间,即动画播放速度按照设定的来");
        }
        if (buff.playTime > 0)
        {
            buff.origin = EditorGUILayout.DelayedFloatField("动画原时长", buff.origin, GUILayout.Width(400), GUILayout.Height(20));
        }
        buff.anim_boolValue = EditorGUILayout.DelayedTextField("bool值触发器名", buff.anim_boolValue, GUILayout.Width(400), GUILayout.Height(20));
        buff.boolValue = EditorGUILayout.Toggle("Bool触发器值",buff.boolValue, GUILayout.Width(400), GUILayout.Height(20));
        buff.anim_triggerValue = EditorGUILayout.DelayedTextField("trigger类型触发器名", buff.anim_triggerValue, GUILayout.Width(400), GUILayout.Height(20));
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

