using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[EditorItemConfig(typeof(ItemUseCondition_LevelLimit))]
public class Editor_ItemUseCondition_LevelLimit : IEditorItemConfig
{
    public void DrawingCondition(IItemUseCondition itemUseCondition)
    {
        ItemUseCondition_LevelLimit itemUseCondition_LevelLimit = itemUseCondition as ItemUseCondition_LevelLimit;
        if (itemUseCondition_LevelLimit == null)
        {
            UnityEngine.Debug.Log("转换失败!");
            return;
        }
        itemUseCondition_LevelLimit.limitLevel = EditorGUILayout.DelayedIntField("    限制等级:", itemUseCondition_LevelLimit.limitLevel, GUILayout.Width(500));
    }
}

