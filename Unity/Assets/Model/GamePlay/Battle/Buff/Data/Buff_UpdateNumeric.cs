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
public class Buff_UpdateNumeric :   BaseBuffData
{
    [LabelText("根据使用者的某个属性,增加属性")]
    [LabelWidth(200)]
    public bool addValueByNumeric;
    [ShowIf("addValueByNumeric")]
    [LabelWidth(200)]
    public NumericType sourceNumeric;
    [ShowIf("addValueByNumeric")]
    [LabelWidth(200)]
    public float coefficient;
    [LabelText("基础增加值")]
    [LabelWidth(200)]
    public float valueAdd;//基础增加值
    [LabelText("目标属性")]
    [LabelWidth(200)]
    public NumericType targetNumeric;
    [HideInInspector]
    [NonSerialized]
    public float updateValue;//用以记录到底给目标角色添加了多少属性
    public override string GetBuffIdType()
    {
        return BuffIdType.UpdateNumeric;
    }
}
