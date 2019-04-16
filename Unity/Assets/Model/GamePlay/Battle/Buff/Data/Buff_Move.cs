using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[LabelText("位移")]
[LabelWidth(150)]
[Serializable]
public class Buff_Move : BaseBuffData
{
    public bool resetDir;//是否重置角色方向,如果是,那么角色方向为当前位置到目标位置
    public float moveDistance;// 位移的距离

    public bool flash;//瞬间移动
    [HideIf("flash")]
    public float moveDuration;

    public override string GetBuffIdType()
    {
        return BuffIdType.Move;
    }

}
