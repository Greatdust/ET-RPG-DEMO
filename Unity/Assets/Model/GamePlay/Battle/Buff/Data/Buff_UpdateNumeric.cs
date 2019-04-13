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
    [LabelText("效果受使用者属性影响")]
    [LabelWidth(220)]
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
    [LabelWidth(120)]
    public float valueAdd;//基础增加值
    [LabelText("成长值")]
    [LabelWidth(120)]
    public float growthValue;// 随技能等级的成长值 
    [LabelText("目标属性")]
    [LabelWidth(120)]
    public NumericType targetNumeric;

    public override string GetBuffIdType()
    {
        return BuffIdType.UpdateNumeric;
    }
}
