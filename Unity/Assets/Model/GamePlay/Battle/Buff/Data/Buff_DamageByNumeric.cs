using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


[Serializable]
public struct Buff_DamageByNumeric :   IBuffData
{
    public NumericType numericType;
    public float baseCoff;//系数
    public GameCalNumericTool.DamageType damageType;

    public float growthCoff;// 系数随技能等级的成长值 


    public string GetBuffIdType()
    {
        return BuffIdType.DamageByNumeric;
    }
}
