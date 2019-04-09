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
public class BuffMgrComponentUpdateSystem : UpdateSystem<BuffMgrComponent>
{
    public override void Update(BuffMgrComponent self)
    {
        self.Update();
    }
}

/// <summary>
/// Unit身上管理所有BUFF的组件
/// </summary>
public class BuffMgrComponent : ETModel.Component
{

    public Dictionary<string, BuffGroup> buffGroupDic;

    public List<BuffGroup> updateList;//保存duration>0的BuffGroup
    public List<BuffGroup> dontRemoveOnBattleEndBuffGroups;

    public RemoveBuffGroupEvent removeBuffGroupEvent;

    private const float calSpan = 1;

    public void Awake()
    {
        buffGroupDic = new Dictionary<string, BuffGroup>();
        updateList = new List<BuffGroup>();
        dontRemoveOnBattleEndBuffGroups = new List<BuffGroup>();
        removeBuffGroupEvent = new RemoveBuffGroupEvent();
        removeBuffGroupEvent.buffMgr = this;
    }

    public void Update()
    {
        if (BattleMgrComponent.Instance == null) return;
        if (BattleMgrComponent.Instance.Waiting) return;
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
        if (dontRemoveOnBattleEndBuffGroups.Count > 0)
        {
            for (int i = dontRemoveOnBattleEndBuffGroups.Count - 1; i >= 0; i--)
            {

                if (!DealWithBuffGroup(dontRemoveOnBattleEndBuffGroups[i]))
                {
                    dontRemoveOnBattleEndBuffGroups.RemoveAt(i);
                }
            }
        }

    }

    bool DealWithBuffGroup(BuffGroup buffGroup)
    {
        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(buffGroup.GetHashCode());
        timer.timing += Time.deltaTime;
        if (timer.timing >= calSpan)
        {
            timer.timing = 0;
            timer.remainTime -= calSpan;
        }
        if (buffGroup.buffList.Count > 0)
        {
            foreach (var v in buffGroup.buffList)
            {
                switch (v)
                {
                    case Buff_DOT dot:
                        GameCalNumericTool.CalDotDamage(GetParent<Unit>(), dot);
                        break;

                    default:
                        break;
                }
            }
        }

        if (timer.remainTime <= 0)
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


    public async void AddBuffGroup(string groupId, BuffGroup group)
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

            BuffGroup newGroup = BuffGroupHelper.GetNewBuffGroup(group);
            newGroup.SetBuffGroupId(group.BuffGroupId);
            buffGroupDic[groupId] = newGroup;
            Unit target = Parent as Unit;
            Unit source = null;

            if (newGroup.sourceUnitId != 0)
                source = UnitComponent.Instance.Get(newGroup.sourceUnitId);
            else
                source = target;
            newGroup.OnBuffGroupAdd(source, target);
            if (newGroup.duration > 0)
            {
                if (newGroup.removeOnBattleEnd)
                    updateList.Add(newGroup);
                else
                    dontRemoveOnBattleEndBuffGroups.Add(newGroup);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
    public void RemoveGroup(string groupId)
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
        dontRemoveOnBattleEndBuffGroups.Clear();
        buffGroupDic.Clear();
    }

}

public class RemoveBuffGroupEvent : AEvent<string>
{
    public BuffMgrComponent buffMgr;
    public override void Run(string groupId)
    {
        if (buffMgr != null)
        {
            if (buffMgr.buffGroupDic.ContainsKey(groupId))
            {
                buffMgr.RemoveGroup(groupId);
            }
        }
    }
}

