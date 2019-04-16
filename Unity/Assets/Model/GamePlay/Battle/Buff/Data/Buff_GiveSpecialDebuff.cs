using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[LabelText("给予特殊DEBUFF")]
[LabelWidth(150)]
[Serializable]
public class Buff_GiveSpecialDebuff : BaseBuffData
{
    public RestrictionType restrictionType;

    public int aimStackNum ;//叠加多少层之后才触发特殊效果

    public override string GetBuffIdType()
    {
        return BuffIdType.GiveSpecialDebuff;
    }
}
