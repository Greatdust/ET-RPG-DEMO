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
public struct BuffGroup
{
    private long buffGroupId;

    [HideInInspector]
    [NonSerialized]
    public long sourceUnitId;// 添加到一个Unit的BuffMgr上时,这个用以记录这个buffGroup的来源

    [LabelText("Buff组名称")]
    [LabelWidth(150)]
    public string buffGroupName;
    [LabelText("Buff组描述")]
    [LabelWidth(150)]
    public string buffGroupDesc;//描述
    [Header("-1代表持续到BUFF组被解除,0代表瞬间完成.大于0代表持续一段时间")]
    [LabelText("Buff持续时间")]
    [LabelWidth(150)]
    public float duration ;

    public List<IBuffData> buffList;

    public long BuffGroupId
    {
        get
        {
            if (buffGroupId == 0)
                buffGroupId = IdGenerater.GenerateId();
            return buffGroupId;
        }
        set => buffGroupId = value;
    }

    public void AddBuff(IBuffData baseBuffData)
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

    void OnBuffAdd(IBuffData v, Unit source, Unit target)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.GetBuffIdType());
        IBuffActionWithGetInputHandler buffActionWithGetInputHandler = baseBuffHandler as IBuffActionWithGetInputHandler;
        buffActionWithGetInputHandler?.ActionHandle(v, source, new List<IBufferValue>() { new BufferValue_TargetUnits() { targets = new Unit[1] { target } } });
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

    void Remove(IBuffData v,Unit source,Unit target)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.GetBuffIdType());
        IBuffRemoveHanlder buffRemoveHanlder = baseBuffHandler as IBuffRemoveHanlder;
        buffRemoveHanlder?.Remove(v, source, new List<IBufferValue>() { new BufferValue_TargetUnits() { targets = new Unit[1] { target } } });
    }

    public void OnBuffGroupRefresh(IBuffData oldBuff, IBuffData newBuff, Unit source, Unit target)
    {
        Remove(oldBuff, source, target);
        OnBuffAdd(newBuff, source, target);
    }

}

