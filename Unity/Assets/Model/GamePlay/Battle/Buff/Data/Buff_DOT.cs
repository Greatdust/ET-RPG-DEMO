using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class Buff_DOT : BaseBuffData
{
    public NumericType numericType;//DOT的伤害和哪个数值类型有关
    public float coefficient;//系数
    public GameCalNumericTool.DamageType damageType;

    [NonSerialized]
    public float damageFinalAddPct;//Buff被附加时,使用方的最终伤害加成率
    [NonSerialized]
    public int damageValue;//造成的伤害值,无法直接编辑,通过计算

    public BuffStackType BuffStackType { get; set; } = BuffStackType.独立;
    public int stackLimit;//叠加层数上限

    public override string GetBuffIdType()
    {
        return BuffIdType.DOT;
    }
}
