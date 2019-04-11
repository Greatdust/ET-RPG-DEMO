using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

//管理游戏内的全局变量
public class GameGlobalVariable : SerializedScriptableObject
{
    public Dictionary<string, int> intVarDic = new Dictionary<string, int>();
    public Dictionary<string, float> floatVarDic = new Dictionary<string, float>();
    public Dictionary<string, string> stringVarDic = new Dictionary<string, string>();
    public Dictionary<string, bool> boolVarDic = new Dictionary<string, bool>();
}

