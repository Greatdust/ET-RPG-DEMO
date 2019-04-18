using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

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

public static class BuffMgrComponentSystem
{

    public static void Awake(this BuffMgrComponent self)
    {
        self.buffGroupDic = new Dictionary<long, BuffGroup>();
        self.updateList = new List<BuffGroup>();
    }

    public static void FixedUpdate(this BuffMgrComponent self)
    {
        if (self.updateList.Count > 0)
        {
            for (int i = self.updateList.Count - 1; i >= 0; i--)
            {

                if (!self.DealWithBuffGroup(self.updateList[i]))
                {
                    self.updateList.RemoveAt(i);
                }
            }
        }

    }

    static bool DealWithBuffGroup(this BuffMgrComponent self,BuffGroup buffGroup)
    {
        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(buffGroup.GetHashCode());
        long now = TimeHelper.Now();
        if (now - timer.timing >= BuffMgrComponent.calSpan)
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
                        GameCalNumericTool.CalDotDamage(buffGroup.sourceUnitId, self.GetParent<Unit>(), dot);
                        break;

                    default:
                        break;
                }
            }
        }

        if (timer.interval <= 0)
        {
            self.RemoveGroup(buffGroup.BuffGroupId);
            TimeSpanHelper.Remove(buffGroup.GetHashCode());
            return false;
        }
        else
        {
            return true;
        }
    }


    public static async void AddBuffGroup(this BuffMgrComponent self, long groupId, BuffGroup group)
    {
        try
        {


            //刷新BUFF,暂时没做叠加

            if (self.buffGroupDic.ContainsKey(groupId))
            {
                Log.Debug("移除原Buff");
                self.RemoveGroup(groupId);
                await TimerComponent.Instance.WaitAsync(0.1F);//延迟一下防止卡顿
            }

            BuffGroup newGroup = group;
            newGroup.BuffGroupId = group.BuffGroupId;
            self.buffGroupDic[groupId] = newGroup;
            if (newGroup.duration > 0)
            {
                self.updateList.Add(newGroup);
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
    public static void RemoveGroup(this BuffMgrComponent self, long groupId)
    {

        BuffGroup group;
        if (!self.buffGroupDic.TryGetValue(groupId, out group))
        {
            return;
        }
        Unit target = self.Parent as Unit;
        Unit source = null;
        if (group.sourceUnitId != 0)
            source = UnitComponent.Instance.Get(group.sourceUnitId);
        else
            source = target;
        group.OnBuffGroupRemove(source, target);
        self.buffGroupDic.Remove(groupId);
    }




    public static void ClearBuffGroupOnBattleEnd(this BuffMgrComponent self)
    {
        if (self.updateList.Count > 0)
        {
            for (int i = 0; i < self.updateList.Count; i++)
            {

                self.RemoveGroup(self.updateList[i].BuffGroupId);
            }
            self.updateList.Clear();
        }
        //TODO:后续如果出现诸如冰冻等限制类类型的DEBUFF,会在这里统一去除
    }
}

