using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[Serializable]
public struct Buff_DirectDamage : IBuffData
{
    public int damageValue;
    public GameCalNumericTool.DamageType damageType;
    public int growthValue;// 随技能等级的成长值 

    public string GetBuffIdType()
    {
        return BuffIdType.DirectDamage;
    }
}
