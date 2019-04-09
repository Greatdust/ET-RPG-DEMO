using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

/// <summary>
/// 一组BUFF,用来组成玩家眼中的装备/道具/技能等等附加的持续性效果
/// </summary>
[Serializable]
public class BuffGroup
{
    [NonSerialized]
    private static int groupIdValue;
    public static string GroupIdGenerater()
    {
        return (TimeHelper.ClientNowSeconds() << 16 + ++groupIdValue).ToString();
    }
    private string buffGroupId;

    [HideInInspector]
    public long sourceUnitId;

    [LabelText("Buff组名称")]
    [LabelWidth(150)]
    public string buffGroupName;
    [LabelText("Buff组描述")]
    [LabelWidth(150)]
    public string buffGroupDesc;//描述
    [Header("-1代表持续到BUFF组被解除,0代表瞬间完成.大于0代表持续一段时间")]
    [LabelText("Buff持续时间")]
    public float duration = -1;
    [LabelText("战斗结束后是否要剔除")]
    [LabelWidth(150)]
    public bool removeOnBattleEnd;


    public List<BaseBuffData> buffList = new List<BaseBuffData>();

    public BuffGroup GetMemberwiseClone()
    {
        return this.MemberwiseClone() as BuffGroup;
    }

    public string BuffGroupId {
        get
        {
            if (string.IsNullOrEmpty(buffGroupId))
                buffGroupId = BuffGroup.GroupIdGenerater();
            return buffGroupId;
        }
            set => buffGroupId = value;
    }

    public void SetBuffGroupId(string id)
    {
        buffGroupId = id;
    }

    public void AddBuff(BaseBuffData baseBuffData)
    {
        buffList.Add(baseBuffData);
    }

    public int IsExist(string buffId)
    {
        int num = 0;
        foreach (var v in buffList)
        {
            if (v.GetBuffIdType() == buffId)
            {
                num++;
            }
        }
        return num;
    }

    /// <summary>
    /// 禁止BUFFMgrComponent以外的地方调用它
    /// </summary>
    /// <param name="unitId"></param>
    public void OnBuffGroupAdd(Unit source, Unit target)
    {
        if (buffList.Count>0)
        {
            foreach (var v in buffList)
            {
                OnBuffAdd(v, source, target);
            }
        }
    }

    void OnBuffAdd(BaseBuffData v, Unit source, Unit target)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.GetBuffIdType());
        IBuffActionWithGetInputHandler buffActionWithGetInputHandler = baseBuffHandler as IBuffActionWithGetInputHandler;
        buffActionWithGetInputHandler?.ActionHandle(v, source, new List<IBuffReturnedValue>() { new BuffReturnedValue_TargetUnit() { target = target, playSpeedScale =1 } });
    }

    public void OnBuffGroupRemove(Unit source, Unit target)
    {
        if (buffList.Count > 0)
        {
            foreach (var v in buffList)
            {
                Remove(v, source, target);
            }
        }
    }

    void Remove(BaseBuffData v,Unit source,Unit target)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.GetBuffIdType());
        IBuffRemoveHanlder buffRemoveHanlder = baseBuffHandler as IBuffRemoveHanlder;
        buffRemoveHanlder?.Remove(v, source, new List<IBuffReturnedValue>() { new BuffReturnedValue_TargetUnit() { target = target,playSpeedScale =1 } });
    }

    public void OnBuffGroupRefresh(BaseBuffData oldBuff, BaseBuffData newBuff, Unit source, Unit target)
    {
        Remove(oldBuff, source, target);
        OnBuffAdd(newBuff, source, target);
    }

}

public static class BuffGroupHelper
{

    public static BuffGroup GetNewBuffGroup(BuffGroup newBuffGroup)
    {
        BuffGroup buffGroup = newBuffGroup.GetMemberwiseClone();
        buffGroup.SetBuffGroupId(newBuffGroup.BuffGroupId);
        return buffGroup;

    }

}

