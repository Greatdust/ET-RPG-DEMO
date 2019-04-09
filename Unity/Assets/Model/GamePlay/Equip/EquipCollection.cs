using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class EquipCollection : SerializedScriptableObject
{
    public Dictionary<string, EquipData> equipDic = new Dictionary<string, EquipData>();
}
