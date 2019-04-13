using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
//这个是道具,任务等给的直接永久增加的数值
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
