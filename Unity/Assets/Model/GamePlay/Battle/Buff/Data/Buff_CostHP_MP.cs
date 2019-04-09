using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
[Serializable]
public class Buff_CostHP_MP :   BaseBuffData
{
    public float costHp;
    public float costMp;
    public float costHpInPct;
    public float costMpInPct;

    public override string GetBuffIdType()
    {
        return BuffIdType.CostHPMP;
    }
}
