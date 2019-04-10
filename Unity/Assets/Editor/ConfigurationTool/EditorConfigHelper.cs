using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.Graphs;
using UnityEditorInternal;
using UnityEngine;
using UnityEngine.U2D;

public static class EditorBuffConfigHelper
{


    public static Dictionary<string, Type> BuffTypeDic = new Dictionary<string, Type>();
    public static Dictionary<string, IEditorBuffConfig> BuffConfigDic = new Dictionary<string, IEditorBuffConfig>();

    public static List<string> buffNonTargetList = new List<string>();
    public static List<string> buffHasCollisionEvent = new List<string>();

    private static int index;
    private static string[] buffTypes;
    private static ReorderableList reorderableList;
    private static BaseSkillData.Pipeline_TriggerType selectedPipelineType;
    private static int insertIndex;

    static EditorBuffConfigHelper()
    {
        foreach (var v in typeof(EditorBuffConfigHelper).Assembly.GetTypes())
        {
            var attr_buffConfig = v.GetCustomAttributes(typeof(BuffConfigAttribute), false);
            if (attr_buffConfig != null && attr_buffConfig.Length > 0)
            {
                object o = Activator.CreateInstance(v);
                IEditorBuffConfig editorBuffConfig = o as IEditorBuffConfig;
                BuffConfigAttribute buffConfigAttribute = attr_buffConfig[0] as BuffConfigAttribute;
                BuffConfigDic.Add(buffConfigAttribute.key, editorBuffConfig);
                BuffTypeDic.Add(buffConfigAttribute.key, buffConfigAttribute.buffType);

                var attr_nonTarget = v.GetCustomAttributes(typeof(BuffNonTargetAttribute), false);
                if (attr_nonTarget != null && attr_nonTarget.Length > 0)
                {
                    buffNonTargetList.Add(buffConfigAttribute.key);
                }

                var attr_hasCollisionEvent = v.GetCustomAttributes(typeof(BuffHaveCollisionEventAttribute), false);
                if (attr_hasCollisionEvent != null && attr_hasCollisionEvent.Length > 0)
                {
                    buffHasCollisionEvent.Add(buffConfigAttribute.key);
                }
            }




        }

    }

    public static void DrawBuff(List<BaseBuffData> buffDataList, BaseBuffData baseBuffData)
    {
        using (new IndentLevelScope(3))
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.Width(500)))
            {
                bool b = EditorGUILayout.Foldout(EditorStateHelper.GetFold(baseBuffData.GetHashCode()), baseBuffData.GetBuffIdType() + "  序号:" + buffDataList.IndexOf(baseBuffData),
                    EditorGUIHelper.GetGUIStyle_FoldOut(EditorStyles.foldout, Color.blue));
                EditorStateHelper.SetFold(baseBuffData.GetHashCode(), b);
                if (GUILayout.Button("删除BUFF", GUILayout.Width(150)))
                {
                    buffDataList.Remove(baseBuffData);
                    return;
                }
            }
        }
        if (EditorStateHelper.GetFold(baseBuffData.GetHashCode()))
        {
            using (new IndentLevelScope(4))
            {
                BuffConfigDic[baseBuffData.GetBuffIdType()].DrawingBuff(baseBuffData);
            }
        }
    }


    public static void DrawSkillPipeline(BaseSkillData skillData)
    {

        selectedPipelineType = (BaseSkillData.Pipeline_TriggerType)EditorGUILayout.EnumPopup("选择一种Pipeline逻辑节点的类型: ", selectedPipelineType, GUILayout.Width(400),GUILayout.Height(20));
        insertIndex = EditorGUILayout.IntSlider("选择节点插入的位置序号", insertIndex, 0, skillData.pipelineDatas.Count, GUILayout.Width(600), GUILayout.Height(20));
        if (GUILayout.Button("添加节点", GUILayout.Width(300)))
        {
            BaseSkillData.BasePipelineData basePipelineData = null;
            switch (selectedPipelineType)
            {
                case BaseSkillData.Pipeline_TriggerType.碰撞检测:
                    basePipelineData = new BaseSkillData.Pipeline_Collision();
                    break;
                case BaseSkillData.Pipeline_TriggerType.固定时间:
                    basePipelineData = new BaseSkillData.Pipeline_FixedTime();
                    break;
                case BaseSkillData.Pipeline_TriggerType.循环开始:
                    basePipelineData = new BaseSkillData.Pipeline_CycleStart();
                    break;
                case BaseSkillData.Pipeline_TriggerType.技能结束:
                    basePipelineData = new BaseSkillData.Pipeline_SkillEnd();
                    break;
            }

            skillData.AddPineLine(basePipelineData,insertIndex);
        }

        if (skillData.pipelineDatas != null && skillData.pipelineDatas.First != null)
        {
            using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
            {
                var head = skillData.pipelineDatas.First;
                DrawPipeLineDataNode(head, skillData,0);
            }
        }
    }

    static void DrawPipeLineDataNode(LinkedListNode<BaseSkillData.BasePipelineData> node, BaseSkillData baseSkillData,int index)
    {

        using (new IndentLevelScope(1))
        {
            using (new EditorGUILayout.HorizontalScope(GUILayout.Width(500)))
            {
                bool b = EditorGUILayout.Foldout(EditorStateHelper.GetFold(node.GetHashCode()), "技能逻辑节点\t"+ node.Value.GetTriggerType() + "\tIndex:  " + index);
                EditorStateHelper.SetFold(node.GetHashCode(), b);
                if (GUILayout.Button("删除节点", GUILayout.Width(150)))
                {
                    baseSkillData.RemovePipeLineData(node.Value);
                    return;
                }
            }
            if (EditorStateHelper.GetFold(node.GetHashCode()))
            {
                node.Value.enable = EditorGUILayout.Toggle("是否开启", node.Value.enable, GUILayout.Width(400), GUILayout.Height(20));
                switch (node.Value.GetTriggerType())
                {
                    case BaseSkillData.Pipeline_TriggerType.碰撞检测:
                        DrawCollision(node.Value as BaseSkillData.Pipeline_Collision, baseSkillData);
                        break;
                    case BaseSkillData.Pipeline_TriggerType.固定时间:
                        DrawDelayTime(node.Value as BaseSkillData.Pipeline_FixedTime, baseSkillData);
                        break;
                    case BaseSkillData.Pipeline_TriggerType.循环开始:
                        DrawCycle(node.Value as BaseSkillData.Pipeline_CycleStart, baseSkillData);
                        break;
                    case BaseSkillData.Pipeline_TriggerType.技能结束:
                        DrawSkillEnd(node.Value as BaseSkillData.Pipeline_SkillEnd, baseSkillData);
                        break;
                }
            }


        }
        index++;
        if (node.Next != null)
        {
            DrawPipeLineDataNode(node.Next, baseSkillData,index);
        }
    }

    static void DrawDelayTime(BaseSkillData.Pipeline_FixedTime pipeline_DelayTime, BaseSkillData baseSkillData)
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUIUtility.labelWidth = 250;

            pipeline_DelayTime.delayTime = EditorGUILayout.DelayedFloatField("延迟时间", pipeline_DelayTime.delayTime, GUILayout.Width(400), GUILayout.Height(20));
            pipeline_DelayTime.fixedTime = EditorGUILayout.DelayedFloatField("该阶段总时长", pipeline_DelayTime.fixedTime, GUILayout.Width(400), GUILayout.Height(20));
            DrawBuffInSkill(pipeline_DelayTime, baseSkillData);



        }
    }

    static void DrawCollision(BaseSkillData.Pipeline_Collision pipeline_Collision, BaseSkillData baseSkillData)
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            EditorGUILayout.LabelField("该节点下所有行为都是上一个节点中发射碰撞检测事件所附带的", GUILayout.Height(20));
            DrawBuffInSkill(pipeline_Collision, baseSkillData);
        }
    }

    static void DrawCycle(BaseSkillData.Pipeline_CycleStart pipeline_CycleStart, BaseSkillData baseSkillData)
    {
        using (new EditorGUILayout.VerticalScope(EditorStyles.helpBox))
        {
            pipeline_CycleStart.cycleRange = (byte)EditorGUILayout.IntField("循环执行后续多少个节点:", pipeline_CycleStart.cycleRange, GUILayout.Width(400), GUILayout.Height(20));
            pipeline_CycleStart.repeatCount = (byte)EditorGUILayout.IntField("循环执行多少次:", pipeline_CycleStart.repeatCount, GUILayout.Width(400), GUILayout.Height(20));
        }
    }

    static void DrawSkillEnd(BaseSkillData.Pipeline_SkillEnd pipeline_SkillEnd, BaseSkillData baseSkillData)
    {
        pipeline_SkillEnd.duration = EditorGUILayout.FloatField("收尾时间:", pipeline_SkillEnd.duration, GUILayout.Width(400), GUILayout.Height(20));
    }


    static void DrawBuffInSkill(BaseSkillData.PipelineDataWithBuff dataWithBuffs, BaseSkillData baseSkillData)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            if (buffTypes == null)
                buffTypes = BuffConfigDic.Keys.ToArray();
            index = EditorGUILayout.Popup("选择待添加的事件类型", index, buffTypes, GUILayout.Width(350), GUILayout.Height(20));
            if (GUILayout.Button("添加事件", GUILayout.Width(100)))
            {
                if (dataWithBuffs.buffs == null)
                    dataWithBuffs.buffs = new List<BaseSkillData.BuffInSkill>();
                BaseSkillData.BuffInSkill buffInSkill = new BaseSkillData.BuffInSkill();
                buffInSkill.buffData = Activator.CreateInstance(BuffTypeDic[buffTypes[index]]) as BaseBuffData;
                baseSkillData.AddBuff(dataWithBuffs, buffInSkill);
                return;
            }
        }
        if (dataWithBuffs.buffs != null)
        {
            for (int i = 0; i < dataWithBuffs.buffs.Count; i++)
            {
                var buffInSkill = dataWithBuffs.buffs[i];
                using (new IndentLevelScope(2))
                {
                    BaseBuffData baseBuffData = buffInSkill.buffData as BaseBuffData;
                    bool b = EditorStateHelper.GetFold(buffInSkill.GetHashCode());
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(800)))
                    {

                        b = EditorGUILayout.Foldout(b,
                            string.Format("类型:  {0}, 标志: {1}", baseBuffData.GetBuffIdType(), buffInSkill.buffSignal),
                               EditorGUIHelper.GetGUIStyle_FoldOut(EditorStyles.foldout, Color.blue));

                        EditorStateHelper.SetFold(buffInSkill.GetHashCode(), b);
                        if (GUILayout.Button("删除事件", GUILayout.Width(150)))
                        {
                            baseSkillData.RemoveBuff(dataWithBuffs, buffInSkill);
                            return;
                        }
                        if (buffInSkill.enable)
                        {
                            EditorGUILayout.LabelField(" 是否为基础Buff:  是", EditorGUIHelper.GetGUIStyle(Color.green), GUILayout.Width(200));
                        }
                        else
                        {
                            EditorGUILayout.LabelField(" 是否为基础Buff:  否", EditorGUIHelper.GetGUIStyle(Color.red), GUILayout.Width(200));
                        }
                    }
                    if (b)
                    {

                        buffInSkill.enable = EditorGUILayout.ToggleLeft("是否默认开启(作为基础BUFF):", buffInSkill.enable, GUILayout.Width(400), GUILayout.Height(20));
                        buffInSkill.delayTime = EditorGUILayout.DelayedFloatField("事件执行延迟时间: ", buffInSkill.delayTime, GUILayout.Width(400), GUILayout.Height(20));
                        buffInSkill.buffSignal_GetInput = EditorGUILayout.DelayedTextField("接收哪个事件的输出作为输入", buffInSkill.buffSignal_GetInput, GUILayout.Width(400), GUILayout.Height(20));
                        if (!buffNonTargetList.Contains(baseBuffData.GetBuffIdType()))
                            buffInSkill.TargetType = (BuffTargetType)EditorGUILayout.EnumPopup("Buff目标", buffInSkill.TargetType, GUILayout.Width(300));
                        BuffConfigDic[baseBuffData.GetBuffIdType()].DrawingBuff(baseBuffData);
                    }
                }
            }
        }
    }

    public static void DrawIncrementBuff(BaseSkillData skillData)
    {
        EditorGUILayout.LabelField("功法在配置对应层数需要启用的增量BUFF时,可以在一个天赋上启用多个增量BUFF", GUILayout.Height(20));

        if (GUILayout.Button("添加一个 增量更新",GUILayout.Width(250),GUILayout.Height(20)))
        {
            skillData.AddIncrementBuff(new ActiveSkillData.IncrementUpdate());
        }
        if (skillData.incrementUpdates != null &&skillData.incrementUpdates.Count > 0)
        {
            for (int i = 0; i < skillData.incrementUpdates.Count; i++)
            {
                var incrementUpdate = skillData.incrementUpdates[i];

                bool b = EditorGUILayout.Foldout(EditorStateHelper.GetFold(skillData.incrementUpdates[i].GetHashCode()),"增量更新名: "+ incrementUpdate.incrementName);
                EditorStateHelper.SetFold(skillData.incrementUpdates[i].GetHashCode(), b);
                if (b)
                {
                    incrementUpdate.incrementName = EditorGUILayout.DelayedTextField("增量更新名: ", incrementUpdate.incrementName);
                    if (incrementUpdate.incrementData == null)
                        incrementUpdate.incrementData = new List<ActiveSkillData.IncrementUpdate.Data>();

                    ReorderableList reorderableList = EditorStateHelper.GetReorderableList(incrementUpdate.incrementData.GetHashCode(), incrementUpdate.incrementData, typeof(ActiveSkillData.IncrementUpdate.Data), (rect, index, active, foucs) =>
                    {
                        incrementUpdate.incrementData[index].buffSignal = EditorGUI.TextField(new Rect(rect.position.x, rect.position.y, 300, rect.size.y), "Buff标志:", incrementUpdate.incrementData[index].buffSignal);
                        incrementUpdate.incrementData[index].enable = EditorGUI.Toggle(new Rect(rect.position.x + 300, rect.position.y, 200, rect.size.y), "设置状态:", incrementUpdate.incrementData[index].enable);
                    },
                    addlist =>
                    {
                        incrementUpdate.incrementData.Add(new ActiveSkillData.IncrementUpdate.Data());
                    },
                    removeList =>
                    {
                        incrementUpdate.incrementData.RemoveAt(removeList.index);
                    }
                    );
                    reorderableList.DoLayoutList();
                }
            }
        }
        
    }

    public static Type GetBuffType(string BuffIdType)
    {
        return BuffTypeDic[BuffIdType];
    }


}

public static class EditorBuffActiceConditionHelper
{
    public static Dictionary<string, Type> BuffTypeDic = new Dictionary<string, Type>();
    public static Dictionary<string, IEditorBuffActiveConditionConfig> BuffConfigDic = new Dictionary<string, IEditorBuffActiveConditionConfig>();
    private static int buffactiveConditionIndex;
    private static string[] buffActiveConditions;

    static EditorBuffActiceConditionHelper()
    {
        foreach (var v in typeof(EditorBuffActiceConditionHelper).Assembly.GetTypes())
        {
            var attr_buffConfig = v.GetCustomAttributes(typeof(EditorBuffActiveConditionAttribute), false);
            if (attr_buffConfig != null && attr_buffConfig.Length > 0)
            {
                object o = Activator.CreateInstance(v);
                IEditorBuffActiveConditionConfig conditionConfig = o as IEditorBuffActiveConditionConfig;
                EditorBuffActiveConditionAttribute buffConfigAttribute = attr_buffConfig[0] as EditorBuffActiveConditionAttribute;
                BuffTypeDic.Add(buffConfigAttribute.key, buffConfigAttribute.buffType);
                BuffConfigDic.Add(buffConfigAttribute.key, conditionConfig);
            }
        }
        buffActiveConditions = BuffTypeDic.Keys.ToArray();

    }



    public static void DrawCondition(BaseSkillData baseSkillData)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            buffactiveConditionIndex = EditorGUILayout.Popup("选择的技能激活条件:", buffactiveConditionIndex, buffActiveConditions, GUILayout.Width(400));
            if (GUILayout.Button("添加条件", GUILayout.Width(200)))
            {
                BaseSkillData.IActiveConditionData buffActiveConditionData = Activator.CreateInstance(BuffTypeDic[buffActiveConditions[buffactiveConditionIndex]]) as BaseSkillData.IActiveConditionData;
                baseSkillData.activeConditionDatas.Add(buffActiveConditionData);
            }
        }
        using (new IndentLevelScope(2))
        {
            for (int i = 0; i < baseSkillData.activeConditionDatas.Count; i++)
            {
                var buffActiveConditionData = baseSkillData.activeConditionDatas[i];
                bool b = EditorStateHelper.GetFold(buffActiveConditionData.GetHashCode());
                using (new EditorGUILayout.HorizontalScope(GUILayout.Width(700)))
                {
                    b = EditorGUILayout.Foldout(b, buffActiveConditionData.GetBuffActiveConditionType());
                    EditorStateHelper.SetFold(buffActiveConditionData.GetHashCode(), b);
                    if (GUILayout.Button("删除条件"))
                    {
                        baseSkillData.activeConditionDatas.RemoveAt(i);
                        return;
                    }
                }
                if (b)
                {
                    using (new IndentLevelScope(3))
                    {
                        BuffConfigDic[buffActiveConditionData.GetBuffActiveConditionType()].DrawCondition(buffActiveConditionData);
                    }
                }
            }
        }
    }

}


public static class EditorItemConfigHelper
{
    public static Dictionary<Type, IEditorItemConfig> ItemConfigDic = new Dictionary<Type, IEditorItemConfig>();

    static EditorItemConfigHelper()
    {
        foreach (var v in typeof(EditorItemConfigAttribute).Assembly.GetTypes())
        {
            var attrs1 = v.GetCustomAttributes(typeof(EditorItemConfigAttribute), false);
            if (attrs1 != null && attrs1.Length > 0)
            {
                object o = Activator.CreateInstance(v);
                EditorItemConfigAttribute editorItemConfigAttribute = attrs1[0] as EditorItemConfigAttribute;
                IEditorItemConfig editorItemConfig = o as IEditorItemConfig;
                ItemConfigDic.Add(editorItemConfigAttribute.key, editorItemConfig);
            }
        }

    }

    public static void DrawCondition(IItemUseCondition itemUseCondition)
    {
        ItemConfigDic[itemUseCondition.GetType()].DrawingCondition(itemUseCondition);
    }

}

public static class EditorSpriteConfigHelper
{
    private static Sprite euqipIconPreview;
    public static void Display(ref string abPrefixName, ref string spriteName)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField("装备图片所在图集AB包前缀名:", GUILayout.Width(200));
            abPrefixName = EditorGUILayout.DelayedTextField(abPrefixName, GUILayout.Width(200), GUILayout.Height(20));
            EditorGUILayout.LabelField("", GUILayout.Width(50));
            spriteName = EditorGUILayout.DelayedTextField("装备图片名字:", spriteName, GUILayout.Width(300), GUILayout.Height(20));
        }
        if (GUILayout.Button("测试图片是否可以正确加载", GUILayout.Width(300)))
        {
            if (string.IsNullOrEmpty(abPrefixName) || string.IsNullOrEmpty(spriteName))
            {
                EditorUtility.DisplayDialog("操作失误", "装备图片名或者图片所在图集名为空!", "确定");
            }
            else
            {
                string[] AssetsPath = AssetDatabase.GetAssetPathsFromAssetBundle(abPrefixName.ToLower() + ".unity3d");
                if (AssetsPath == null)
                {
                    EditorUtility.DisplayDialog("操作失误", "装备图片所在图集AB包不存在!", "确定");
                }
                else
                {
                    euqipIconPreview = (AssetDatabase.LoadMainAssetAtPath(AssetsPath[0]) as SpriteAtlas).GetSprite(spriteName);
                    if (euqipIconPreview == null)
                    {
                        EditorUtility.DisplayDialog("操作失误", "图片名字填写错误! 图集中没有该名字的图片!", "确定");
                    }
                    else
                    {
                        EditorUtility.DisplayDialog("操作成功!", "成功获取对应图片!" + spriteName, "确定");
                    }
                }
            }
        }
    }
}

public static class EditorBuffGroupHelper
{
    private static int selectedBuffIndex_ToAdd = 0;
    public static void DrawBuffGroup(BuffGroup buffGroup,string[] stringArray,bool buffGroupOnlyOneNumeric)
    {
        if (buffGroup == null)
            buffGroup = new BuffGroup();
        buffGroup.duration = EditorGUILayout.DelayedFloatField("buff持续时间:", buffGroup.duration, GUILayout.Width(500));
        EditorGUILayout.LabelField("\t-1代表持续到BUFF被解除,正数代表持续多少秒. " +
            "\n\t一般装备,被动技能之类的效果保持-1就行了." +
            "\n\t主动技能的效果则填具体多少秒." +
            "\n\t道具如果是立即回复的,填0," +
            "\n\t如果是逐渐回复的,选择对应的每秒恢复数值,并填具体持续多少秒", GUILayout.Height(120));
        using (new IndentLevelScope(2))
        {
            using (new EditorGUILayout.HorizontalScope())
            {
                EditorGUILayout.LabelField("选择一个BUFF加入该Buff组中", GUILayout.Width(200));
                selectedBuffIndex_ToAdd = EditorGUILayout.Popup(selectedBuffIndex_ToAdd, stringArray, GUILayout.Width(150));
                if (GUILayout.Button("添加BUFF", GUILayout.Width(150)))
                {
                    string value = stringArray[selectedBuffIndex_ToAdd];
                    if (buffGroupOnlyOneNumeric && value != BuffIdType.EmitEffectInSkill && buffGroup.IsExist(value) >= 1)
                    {
                        EditorUtility.DisplayDialog("错误!", "这里的每一个Buff组只能有一条数值/特性!", "确定!");
                    }
                    else
                    {
                        BaseBuffData buff = Activator.CreateInstance(EditorBuffConfigHelper.GetBuffType(value)) as BaseBuffData;
                        if (buff == null)
                            Debug.Log("nULL!!!!");
                        buffGroup.AddBuff(buff);
                    }
                }
            }
        }
        if (buffGroup != null && buffGroup.buffList.Count > 0)
        {
            for (int j = 0; j < buffGroup.buffList.Count; j++)
            {
                BaseBuffData baseBuffData = buffGroup.buffList[j];
                using (new IndentLevelScope(3))
                {
                    using (new EditorGUILayout.HorizontalScope(GUILayout.Width(500)))
                    {
                        bool b = EditorGUILayout.Foldout(EditorStateHelper.GetFold(baseBuffData.GetHashCode()), baseBuffData.GetBuffIdType(),
                            EditorGUIHelper.GetGUIStyle_FoldOut(EditorStyles.foldout, Color.blue));
                        EditorStateHelper.SetFold(baseBuffData.GetHashCode(), b);
                        if (GUILayout.Button("删除BUFF", GUILayout.Width(150)))
                        {
                            buffGroup.buffList.RemoveAt(j);
                            return;
                        }
                    }
                }
                if (EditorStateHelper.GetFold(baseBuffData.GetHashCode()))
                {
                    using (new IndentLevelScope(4))
                    {
                        EditorBuffConfigHelper.BuffConfigDic[baseBuffData.GetBuffIdType()].DrawingBuff(baseBuffData);
                    }
                }
            }
        }
    }
}