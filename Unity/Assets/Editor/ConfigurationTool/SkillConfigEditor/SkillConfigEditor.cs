using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

public class SkillConfigEditor : EditorWindow
{
    public static SkillConfigEditor instance;


    public BaseSkillData selectedSkill;

    public SkillCollection mSkillCollection;

    private Rect toolbarGUIRect;
    private Rect settingGUIRect;

    private int selectedIndex;
    private int oldSelectedIndex;
    private string newSkillId;

    private int selectedBuffIndex_ToAdd;

    public const string collectionFullPath = "Assets/Res/GameConfig/SkillConfig/SkillCollection.asset";


    [MenuItem("配置工具/技能配置")]
    static void ShowWindow()
    {
        EditorWindow.GetWindow<SkillConfigEditor>(false, "技能配置", true);
    }


    private void OnEnable()
    {
        instance = this;

        toolbarGUIRect = new Rect(0, 0, position.width, 75);
        settingGUIRect = new Rect(0, toolbarGUIRect.height, position.width, position.height - toolbarGUIRect.height);
        if (!File.Exists(collectionFullPath))
        {
            mSkillCollection = ScriptableObject.CreateInstance<SkillCollection>();
            AssetDatabase.CreateAsset(mSkillCollection, collectionFullPath);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
        }
        else
        {
            mSkillCollection = AssetDatabase.LoadAssetAtPath<SkillCollection>(collectionFullPath);
        }
        Init();
    }
    void Init()
    {
        selectedIndex = 0;
    }

    private void OnGUI()
    {
        GUI.skin.font = EditorGUIHelper.DefaultFont;
        Content();
        SaveData();
    }

    void SaveData()
    {
        if (mSkillCollection != null && GUI.changed)
        {
            Debug.Log("数据已保存");
            EditorUtility.SetDirty(mSkillCollection);
        }
    }

    private int editTypeInToolBar;
    private string[] editTypeNamesInToolBar = new string[] { "配置主动技能", "配置被动技能"};

    void Content()
    {

        toolbarGUIRect = new Rect(0, 0, position.width, 100);
        GUILayout.BeginArea(toolbarGUIRect, EditorStyles.helpBox);
        EditorGUI.BeginChangeCheck();
        editTypeInToolBar = GUILayout.Toolbar(editTypeInToolBar, editTypeNamesInToolBar, GUILayout.Width(800));
        if (EditorGUI.EndChangeCheck())
        {
            selectedIndex = 0;
            selectedSkill = null;
        }
        if (editTypeInToolBar == 0)
        {
            ActiveSkillToolBar();
        }
        else
        {
            PassiveSkillToolBar();
        }


        GUILayout.EndArea();

        SkillSettingGUI();


    }

    void ActiveSkillToolBar()
    {
        EditorGUILayout.LabelField("本工具由弈剑游戏工作室程序AE倾力打造.有任何疑问请联系QQ:210379417");
        if (mSkillCollection.activeSkillDataDic != null && mSkillCollection.activeSkillDataDic.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("选择已有技能:", GUILayout.Width(150));
            var Contents = mSkillCollection.activeSkillDataDic.Keys.ToArray<string>();
            selectedIndex = EditorGUILayout.Popup(selectedIndex, Contents, GUILayout.Width(200));
            selectedSkill = mSkillCollection.activeSkillDataDic[Contents[selectedIndex]];
            if (GUILayout.Button("删除技能", GUILayout.Width(200)))
            {
                if (selectedSkill != null)
                {
                    selectedIndex = 0;
                    mSkillCollection.activeSkillDataDic.Remove(Contents[selectedIndex]);
                    SaveData();
                    selectedSkill = null;
                    Debug.Log("删除技能成功!");
                    return;
                }
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("创建新技能,id为:", GUILayout.Width(150));
        newSkillId = EditorGUILayout.DelayedTextField(newSkillId, GUILayout.Width(200), GUILayout.Height(20));
        if (GUILayout.Button("新建技能", GUILayout.Width(200)))
        {
            if (string.IsNullOrWhiteSpace(newSkillId))
            {
                EditorUtility.DisplayDialog("提示", "技能名不能为空!", "确定");
            }
            else
            {
                if (!mSkillCollection.activeSkillDataDic.ContainsKey(newSkillId))
                {
                    mSkillCollection.activeSkillDataDic.Add(newSkillId, new ActiveSkillData());
                    mSkillCollection.activeSkillDataDic[newSkillId].skillId = newSkillId;
                    SaveData();
                    Debug.Log("新建技能成功!新技能id为  " + newSkillId);
                    newSkillId = null;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该技能名已存在,无法新建!", "确定");
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();


        EditorGUILayout.EndHorizontal();
    }

    void PassiveSkillToolBar()
    {
        EditorGUILayout.LabelField("本工具由弈剑游戏工作室程序AE倾力打造.有任何疑问请联系QQ:210379417");
        if (mSkillCollection.passiveSkillDataDic != null && mSkillCollection.passiveSkillDataDic.Count > 0)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("选择已有技能:", GUILayout.Width(150));
            var Contents = mSkillCollection.passiveSkillDataDic.Keys.ToArray<string>();
            selectedIndex = EditorGUILayout.Popup(selectedIndex, Contents, GUILayout.Width(200));
            selectedSkill = mSkillCollection.passiveSkillDataDic[Contents[selectedIndex]];
            if (GUILayout.Button("删除技能", GUILayout.Width(200)))
            {
                if (selectedSkill != null)
                {
                    selectedIndex = 0;
                    mSkillCollection.passiveSkillDataDic.Remove(Contents[selectedIndex]);
                    SaveData();
                    selectedSkill = null;
                    Debug.Log("删除技能成功!");
                    return;
                }
            }
            GUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("创建新技能,id为:", GUILayout.Width(150));
        newSkillId = EditorGUILayout.DelayedTextField(newSkillId, GUILayout.Width(200), GUILayout.Height(20));
        if (GUILayout.Button("新建技能", GUILayout.Width(200)))
        {
            if (string.IsNullOrWhiteSpace(newSkillId))
            {
                EditorUtility.DisplayDialog("提示", "技能名不能为空!", "确定");
            }
            else
            {
                if (!mSkillCollection.passiveSkillDataDic.ContainsKey(newSkillId))
                {
                    mSkillCollection.passiveSkillDataDic.Add(newSkillId, new PassiveSkillData());
                    mSkillCollection.passiveSkillDataDic[newSkillId].skillId = newSkillId;
                    SaveData();
                    Debug.Log("新建技能成功!新技能id为  " + newSkillId);
                    newSkillId = null;
                }
                else
                {
                    EditorUtility.DisplayDialog("提示", "该技能名已存在,无法新建!", "确定");
                }
            }
        }
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        EditorGUILayout.Space();


        EditorGUILayout.EndHorizontal();
    }


    private int editType;
    private string[] editTypeNames = new string[] { "配置技能基本参数","配置技能使用条件", "编辑技能Pipeline数据", "编辑技能增量BUFF" };
    void SkillSettingGUI()
    {
        if (selectedSkill == null) return;
        settingGUIRect = new Rect(0, toolbarGUIRect.height, position.width, position.height - toolbarGUIRect.height);
     
        GUILayout.BeginArea(settingGUIRect, EditorStyles.helpBox);

        EditorGUILayout.LabelField("技能编辑器中如果要给技能添加数值改变,持续伤害之类的BUFF,请在技能结束时添加BUFF中添加");
        editType = GUILayout.Toolbar(editType, editTypeNames, GUILayout.Width(800));

        switch (editType)
        {
            case 0:
                SkillBaseDataConfig();
                break;
            case 1:
                ActiveConditionConfig(selectedSkill);
                break;
            case 2:
                DrawPipelineData();
                break;
            case 3:
                IncrementBuffConfig();
                break;
            default:
                break;
        }
        GUILayout.EndArea();
    }

    private string[] passiveEvents;

    void SkillBaseDataConfig()
    {
    
        EditorGUIUtility.labelWidth = 250;
        EditorGUILayout.LabelField("当前技能Id", selectedSkill.skillId);
        EditorGUILayout.Space();
        selectedSkill.skillName = EditorGUILayout.DelayedTextField("技能名：", selectedSkill.skillName, GUILayout.Width(400));
        EditorGUILayout.Space();
        selectedSkill.skillAssetsABName = EditorGUILayout.DelayedTextField("技能资源AB包前缀名：", selectedSkill.skillAssetsABName, GUILayout.Width(400));
        EditorGUILayout.Space();
        selectedSkill.mainTargetType = (BuffTargetType)EditorGUILayout.EnumPopup("技能主目标类型:", selectedSkill.mainTargetType, GUILayout.Width(400));
        if (selectedSkill.SkillType == SkillType.Active)
        {
            ActiveSkillData activeSkillData = selectedSkill as ActiveSkillData;
            EditorGUILayout.Space();
            activeSkillData.isNormalAttack = EditorGUILayout.Toggle("是否是普通攻击:", activeSkillData.isNormalAttack, GUILayout.Width(400));
            EditorGUILayout.Space();
            activeSkillData.timeImpact = EditorGUILayout.DelayedFloatField("技能增加读条时间：", activeSkillData.timeImpact, GUILayout.Width(400));
            EditorGUILayout.Space();
            activeSkillData.activeSkillTag = (ActiveSkillTag)EditorGUILayout.EnumPopup("技能标签: ", activeSkillData.activeSkillTag, GUILayout.Width(400));
        }
        else
        {
            PassiveSkillData passiveSkillData = selectedSkill as PassiveSkillData;
            EditorGUILayout.Space();
            if (passiveEvents == null)
            {
                FieldInfo[] fieldInfos = typeof(PassiveSkillListenedEventType).GetFields();
                passiveEvents = new string[fieldInfos.Length];
                for (int i = 0; i < passiveEvents.Length; i++)
                {
                    passiveEvents[i] = fieldInfos[i].GetValue(null) as string;
                }
            }
            using (new EditorGUILayout.HorizontalScope())
            {
                passiveSkillData.listenToEvent = EditorGUILayout.Toggle("效果是动态改变的:", passiveSkillData.listenToEvent, GUILayout.Width(400));
                if (passiveSkillData.listenToEvent)
                {
                    int selectedIndex = EditorStateHelper.GetSelectedIndex(passiveSkillData.GetHashCode());
                    if (passiveEvents[selectedIndex] != passiveSkillData.eventIdType)
                    {
                        for (int i = 0; i < passiveEvents.Length; i++)
                        {
                            if (passiveEvents[i] == passiveSkillData.eventIdType)
                            {
                                selectedIndex = i;
                                break;
                            }
                            selectedIndex = 0;
                        }
                    }
                    EditorStateHelper.SetSelectedIndex(passiveSkillData.GetHashCode(), EditorGUILayout.Popup(selectedIndex, passiveEvents, GUILayout.Width(400)));
                    passiveSkillData.eventIdType = passiveEvents[selectedIndex];
                }
            }

        }
    }
    private Vector2 scrollData;
    private Vector2 incrementBuffScrollData;

    void DrawPipelineData()
    {
        using (var scroll = new EditorGUILayout.ScrollViewScope(scrollData))
        {
            scrollData = scroll.scrollPosition;
            EditorBuffConfigHelper.DrawSkillPipeline(selectedSkill);
        }
    }
    void ActiveConditionConfig(BaseSkillData baseSkillData)
    {
        EditorBuffActiceConditionHelper.DrawCondition(baseSkillData);

    }
    void IncrementBuffConfig()
    {
        using (var scroll = new EditorGUILayout.ScrollViewScope(incrementBuffScrollData))
        {
            incrementBuffScrollData = scroll.scrollPosition;
            EditorBuffConfigHelper.DrawIncrementBuff(selectedSkill);
        }
    }

}

