using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class Buff_GiveNumeric : BaseBuffData
{
    public NumericType numericType;
    public float value;

    public override string GetBuffIdType()
    {
        return BuffIdType.GiveNumeric;
    }
}
