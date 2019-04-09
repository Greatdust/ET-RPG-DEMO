using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[LabelText("百分比伤害")]
[LabelWidth(150)]
[Serializable]
public class Buff_DamageByNumeric :   BaseBuffData
{
    public NumericType numericType;
    public float coefficient;//系数
    public GameCalNumericTool.DamageType damageType;




    public override string GetBuffIdType()
    {
        return BuffIdType.DamageByNumeric;
    }
}
