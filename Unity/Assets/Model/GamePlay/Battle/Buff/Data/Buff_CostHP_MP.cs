using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
[LabelText("消耗")]
[LabelWidth(100)]
[Serializable]
public class Buff_CostHP_MP :   BaseBuffData
{
    public float costHp;
    public float costMp;
    public float costHpInPct;
    public float costMpInPct;

    public float growthPct;// 随技能等级的成长值 ,这里是在基础的消耗上*一个百分比

    public override string GetBuffIdType()
    {
        return BuffIdType.CostHPMP;
    }
}
