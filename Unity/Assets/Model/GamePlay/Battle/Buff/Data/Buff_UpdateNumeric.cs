using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[LabelText("改变角色属性")]
[LabelWidth(150)]
[Serializable]
public struct Buff_UpdateNumeric :   IBuffData
{

    public bool addValueByNumeric;
    [ShowIf("addValueByNumeric")]
    [LabelWidth(200)]
    public NumericType sourceNumeric;
    [ShowIf("addValueByNumeric")]
    [LabelWidth(200)]
    public float coefficient;
    [ShowIf("addValueByNumeric")]
    [LabelWidth(200)]
    public float growthCoff;// 系数随技能等级的成长值 
    [LabelText("基础增加值")]
    [LabelWidth(200)]
    public float valueAdd;//基础增加值
    [LabelText("成长值")]
    [LabelWidth(200)]
    public float growthValue;// 随技能等级的成长值 
    [LabelText("目标属性")]
    [LabelWidth(200)]
    public NumericType targetNumeric;
    [HideInInspector]
    [NonSerialized]
    public float updateValue;//用以记录到底给目标角色添加了多少属性
    public string GetBuffIdType()
    {
        return BuffIdType.UpdateNumeric;
    }
}
