using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
[Serializable]
public class Buff_Move : BaseBuffData
{
    public bool resetDir;//是否重置角色方向,如果是,那么角色方向为当前位置到目标位置
    public float targetPosOffset;//到目标单位所在位置的偏移距离,方向为使用方位置到目标单位位置
    public bool flash;//瞬间移动
    public float moveDuration;

    public override string GetBuffIdType()
    {
        return BuffIdType.Move;
    }

}
