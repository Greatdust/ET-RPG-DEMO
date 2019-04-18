using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
/// <summary>
/// Unit身上管理所有BUFF的组件
/// </summary>
public class BuffMgrComponent : ETModel.Component
{

    public Dictionary<long, BuffGroup> buffGroupDic;

    public List<BuffGroup> updateList;//保存duration>0的BuffGroup,战斗结束后统一移除.
    //如果是节日,活动等导致的长时间BUFF. 比如新人奖励(等级在50级之前,获得的经验值额外增加50%),或者常驻一个持续一周的副本内属性提升的BUFF
    //这种类型的,在另外个地方,单独管理即可. (监听进入副本的事件,而后给角色添加BUFF)

    public const long calSpan = 1000;



    public override void Dispose()
    {
        if (IsDisposed)
            return;
        base.Dispose();
        updateList.Clear();
        buffGroupDic.Clear();
    }

}

