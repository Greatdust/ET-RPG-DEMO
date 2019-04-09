using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[LabelText("固定伤害")]
[LabelWidth(150)]
[Serializable]
public class Buff_DirectDamage : BaseBuffData
{
    public int damageValue;
    public GameCalNumericTool.DamageType damageType;


    public override string GetBuffIdType()
    {
        return BuffIdType.DirectDamage;
    }
}
