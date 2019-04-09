using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[BuffConfigAttribute(BuffIdType.PlaySound, typeof(Buff_PlaySound))]
[BuffNonTarget]
public class BuffEditor_PlaySound : IEditorBuffConfig,IEditorBuff_TimelineConfig, IEditorBuff_TriggerConfig
{

    public void DrawingBuff(BaseBuffData data)
    {
        Buff_PlaySound buff = data as Buff_PlaySound;

        buff.audioClipName = EditorGUILayout.DelayedTextField("音频名", buff.audioClipName, GUILayout.Width(400), GUILayout.Height(20));
        buff.onlyPlayOnceTime = EditorGUILayout.Toggle("是否只播放一次", buff.onlyPlayOnceTime, GUILayout.Width(400), GUILayout.Height(20));
        if(!buff.onlyPlayOnceTime)
        buff.duration = EditorGUILayout.DelayedFloatField("音频持续时间上限:", buff.duration, GUILayout.Width(400), GUILayout.Height(20));
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

