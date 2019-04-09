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

public class ConfigGraphGUI : GraphGUI
{
}

/// <summary>
/// 缩进等级
/// </summary>
public class IndentLevelScope : GUI.Scope
{
    private int level;
    public IndentLevelScope(int level)
    {
        this.level = level;
        EditorGUILayout.BeginHorizontal();
        GUILayout.Space(level * 20);
        EditorGUILayout.BeginVertical();
    }
    protected override void CloseScope()
    {
        EditorGUILayout.EndVertical();
        EditorGUILayout.EndHorizontal();
    }
}

public struct LineData
{
    public Vector3 startPos;
    public Vector3 endPos;
    public Vector3 startTan;
    public Vector3 endTan;
}
public static class EditorMathfHelper
{
    private static int currNodeId;
    public static LineData GetLineData_Left(Rect from, Rect to)
    {
        LineData data = new LineData();
        data.startPos = from.position + new Vector2(0, from.height/2);
        data.endPos = to.position + new Vector2(to.width , to.height/2);
        data.startTan = data.startPos + Vector3.left * 50;
        data.endTan = data.endPos + Vector3.right * 50;
        return data;
    }
    public static LineData GetLineData_Right(Rect from, Rect to)
    {
        LineData data = new LineData();
        data.startPos = from.position + new Vector2(from.width, from.height / 2);
        data.endPos = to.position + new Vector2(0, to.height / 2);
        data.startTan = data.startPos + Vector3.right * 50;
        data.endTan = data.endPos + Vector3.left * 50;
        return data;
    }
    public static LineData GetLineData_Up(Rect from, Rect to)
    {
        LineData data = new LineData();
        data.startPos = from.position + new Vector2(from.width/2, 0);
        data.endPos = to.position + new Vector2(to.width/2, to.height);
        data.startTan = data.startPos + Vector3.down * 50;//角度正负是顺时针,这里应该反向向上,所以角度是负数
        data.endTan = data.endPos + Vector3.up * 50;
        return data;
    }
    public static LineData GetLineData_Down(Rect from, Rect to)
    {
        LineData data = new LineData();
        data.startPos = from.position + new Vector2(from.width/2, from.height);
        data.endPos = to.position + new Vector2(to.width/2, 0);
        data.startTan = data.startPos + Vector3.up * 50;
        data.endTan = data.endPos + Vector3.down * 50;
        return data;
    }
    public static LineData GetNearestLineData_Vectical(Rect from, Rect to)
    {
        if (from.position.y > to.position.y)
        {
            return GetLineData_Up(from, to);
        }
        else
        {
            return GetLineData_Down(from, to);
        }   
    }
    public static LineData GetNearestLineData_Horizontal(Rect from, Rect to)
    {
        if (from.position.x > to.position.x)
        {
            return GetLineData_Left(from, to);
        }
        else
        {
            return GetLineData_Right(from, to);
        }
    }


}

public static class EditorGUIHelper
{
    private static Font defaultFont;

    public static Font DefaultFont {
        get
        {
            defaultFont = defaultFont?? AssetDatabase.LoadAssetAtPath<Font>("Assets/Font/msyhbd");
            return defaultFont;
        }
    }

    private static GUIStyle uIStyle;
    private static GUIStyle uiStyle_1;

    public static GUIStyle GetGUIStyle(Color color)
    {
        if (uIStyle == null)
            uIStyle = new GUIStyle(EditorStyles.label);
        uIStyle.normal.textColor = color;
        return uIStyle;
    }

    public static GUIStyle GetGUIStyle_FoldOut(GUIStyle style, Color color)
    {
        if (uiStyle_1 == null)
            uiStyle_1 = new GUIStyle(style);
        uiStyle_1.fontStyle = FontStyle.Bold;
        return uiStyle_1;
    }

    public static void Space(int num)
    {
        if (num <= 0) return;
        for (int i = 0; i < num; i++)
        {
            EditorGUILayout.Space();
        }
    }

    public static void HorizontalLabel(int totalWidth,int height,params string[] contents)
    {
        using (new EditorGUILayout.HorizontalScope(GUILayout.Width(totalWidth)))
        {
            for (int i = 0; i < contents.Length; i++)
            {
                EditorGUILayout.LabelField(contents[i], GUILayout.Height(height));
            }
        }
    }

    public static void HorizontalLayout(string label,int width, int height, Action layoutAction)
    {
        using (new EditorGUILayout.HorizontalScope())
        {
            EditorGUILayout.LabelField(label, GUILayout.Width(width), GUILayout.Height(height));
            layoutAction();
        }
    }

}




public static class EditorStateHelper
{
    private static Dictionary<int,bool> foldOutStateDic = new Dictionary<int,bool>();
    private static Dictionary<int, int> selectedIndexDic = new Dictionary<int, int>();
    private static Dictionary<int, Vector2> scrollDataDic = new Dictionary<int, Vector2>();
    private static Dictionary<int, ReorderableList> reorderableListDic = new Dictionary<int, ReorderableList>();

    public static bool GetFold(int hash)
    {
        if (foldOutStateDic.ContainsKey(hash) == false)
        {
            foldOutStateDic[hash] = false;
            return false;
        }
        return foldOutStateDic[hash];
    }

    public static void SetFold(int hash, bool value)
    {
        foldOutStateDic[hash] = value;
    }

    public static int GetSelectedIndex(int hash)
    {
        if (selectedIndexDic.ContainsKey(hash) == false)
        {
            selectedIndexDic[hash] = 0;
            return 0;
        }
        return selectedIndexDic[hash];
    }

    public static void SetSelectedIndex(int hash, int value)
    {
        selectedIndexDic[hash] = value;
    }

    public static Vector2 GetScrollData(int hash)
    {
        if (scrollDataDic.ContainsKey(hash) == false)
        {
            scrollDataDic[hash] = Vector2.zero;
        }
        return scrollDataDic[hash];
    }

    public static void SetScrollData(int hash, Vector2 value)
    {
        scrollDataDic[hash] = value;
    }

    public static ReorderableList GetReorderableList(int hash, System.Collections.IList list,Type type, ReorderableList.ElementCallbackDelegate drawElementCallback, ReorderableList.AddCallbackDelegate addCallbackDelegate, ReorderableList.RemoveCallbackDelegate removeCallbackDelegate)
    {
        if (reorderableListDic.ContainsKey(hash) == false)
        {
            reorderableListDic[hash] = new ReorderableList(list, type, true, false, true, true);
            reorderableListDic[hash].drawElementCallback = drawElementCallback;
            reorderableListDic[hash].onAddCallback = addCallbackDelegate;
            reorderableListDic[hash].onRemoveCallback = removeCallbackDelegate;
        }
        return reorderableListDic[hash];
    }

}
