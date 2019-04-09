using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

[EditorBuffActiveCondition(SkillActiveConditionType.CheckEquip, typeof(SkillActiveCondition_CheckEquip))]

public class BuffActiceConditionEditor_CheckEquip : IEditorBuffActiveConditionConfig
{
    public void DrawCondition(BaseSkillData.IActiveConditionData buffActiveConditionData)
    {
        EditorGUIUtility.labelWidth = 200;
        SkillActiveCondition_CheckEquip checkEquip = buffActiveConditionData as SkillActiveCondition_CheckEquip;
        checkEquip.CheckWeapon = EditorGUILayout.Toggle("检测武器的类型", checkEquip.CheckWeapon, GUILayout.Width(400), GUILayout.Height(20));
        if(checkEquip.CheckWeapon)
        checkEquip.targetWeapon = (WeaponType)EditorGUILayout.EnumPopup("目标武器类型", checkEquip.targetWeapon, GUILayout.Width(400), GUILayout.Height(20));
    }

 
}

