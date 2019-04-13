using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

[ObjectSystem]
public class BuffMgrComponentAwakeSystem : AwakeSystem<BuffMgrComponent>
{
    public override void Awake(BuffMgrComponent self)
    {
        self.Awake();
    }
}

[ObjectSystem]
public class BuffMgrComponentUpdateSystem : FixedUpdateSystem<BuffMgrComponent>
{
    public override void FixedUpdate(BuffMgrComponent self)
    {
        self.FixedUpdate();
    }
}

/// <summary>
/// Unit身上管理所有BUFF的组件
/// </summary>
public class BuffMgrComponent : ETModel.Component
{

    public Dictionary<long, BuffGroup> buffGroupDic;

    public List<BuffGroup> updateList;//保存duration>0的BuffGroup,战斗结束后统一移除.
    //如果是节日,活动等导致的长时间BUFF. 比如新人奖励(等级在50级之前,获得的经验值额外增加50%),或者常驻一个持续一周的副本内属性提升的BUFF
    //这种类型的,在另外个地方,单独管理即可. (监听进入副本的事件,而后给角色添加BUFF)

    private const long calSpan = 1000;

    public void Awake()
    {
        buffGroupDic = new Dictionary<long, BuffGroup>();
        updateList = new List<BuffGroup>();
    }

    public void FixedUpdate()
    {
        if (updateList.Count > 0)
        {
            for (int i = updateList.Count-1; i >=0; i--)
            {

                if (!DealWithBuffGroup(updateList[i]))
                {
                    updateList.RemoveAt(i);
                }
            }
        }

    }

    bool DealWithBuffGroup(BuffGroup buffGroup)
    {
        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(buffGroup.GetHashCode());
        long now = TimeHelper.Now();
        if (now - timer.timing >= calSpan)
        {
            timer.timing = now;
        }
        if (buffGroup.buffList.Count > 0)
        {
            foreach (var v in buffGroup.buffList)
            {
                switch (v)
                {
                    case Buff_DOT dot:
                        GameCalNumericTool.CalDotDamage(buffGroup.sourceUnitId,GetParent<Unit>(), dot);
                        break;

                    default:
                        break;
                }
            }
        }

        if (timer.interval <= 0)
        {
            RemoveGroup(buffGroup.BuffGroupId);
            TimeSpanHelper.Remove(buffGroup.GetHashCode());
            return false;
        }
        else
        {
            return true;
        }
    }


    public async void AddBuffGroup(long groupId, BuffGroup group)
    {
        try
        {


            //刷新BUFF,暂时没做叠加

            if (buffGroupDic.ContainsKey(groupId))
            {
                Log.Debug("移除原Buff");
                RemoveGroup(groupId);
                await TimerComponent.Instance.WaitAsync(0.1F);//延迟一下防止卡顿
            }

            BuffGroup newGroup = group;
            newGroup.BuffGroupId = group.BuffGroupId;
            buffGroupDic[groupId] = newGroup;
            if (newGroup.duration > 0)
            {
                updateList.Add(newGroup);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
    public void RemoveGroup(long groupId)
    {
      
        BuffGroup group;
        if (!buffGroupDic.TryGetValue(groupId,out group))
        {
            return;
        }
        Unit target = Parent as Unit;
        Unit source = null;
        if (group.sourceUnitId != 0)
            source = UnitComponent.Instance.Get(group.sourceUnitId);
        else
            source = target;
        group.OnBuffGroupRemove(source,target);
        buffGroupDic.Remove(groupId);
    }




    public void ClearBuffGroupOnBattleEnd()
    {
        if (updateList.Count > 0)
        {
            for (int i = 0; i < updateList.Count; i++)
            {

                RemoveGroup(updateList[i].BuffGroupId);
            }
            updateList.Clear();
        }
        //TODO:后续如果出现诸如冰冻等限制类类型的DEBUFF,会在这里统一去除
    }


    public override void Dispose()
    {
        if (IsDisposed)
            return;
        base.Dispose();
        updateList.Clear();
        buffGroupDic.Clear();
    }

}

