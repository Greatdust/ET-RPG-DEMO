using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[LabelText("击退")]
[LabelWidth(150)]
[Serializable]
public class Buff_PushBack : BaseBuffData
{
    public float distance;// 击退的距离
    public float moveDuration;// 移动用时

    public override string GetBuffIdType()
    {
        return BuffIdType.PushBack;
    }

}
