using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public struct Buff_DOT : IBuffData
{
    public NumericType numericType;//DOT的伤害和哪个数值类型有关
    public float baseCoff;//系数
    public GameCalNumericTool.DamageType damageType;

    public float growthCoff;// 系数随技能等级的成长值 
    [NonSerialized]
    public int damageValue;//造成的伤害值,无法直接编辑,通过计算

    public BuffStackType BuffStackType;
    public int stackLimit;//叠加层数上限
    public float growthValueWithBuffStack; //每多一层,效果增加多少. 从2层开始算

    public string GetBuffIdType()
    {
        return BuffIdType.DOT;
    }
}
