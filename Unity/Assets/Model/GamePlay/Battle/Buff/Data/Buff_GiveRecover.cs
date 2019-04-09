using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[LabelText("给予恢复")]
[Serializable]
public class Buff_GiveRecover : BaseBuffData
{
    public float hpValue;
    public float hpPct;
    public float mpValue;
    public float mpPct;

    public override string GetBuffIdType()
    {
        return BuffIdType.GiveRecover;
    }
}
