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

    [ListDrawerSettings(ShowItemCount = true)]
    public List<BaseBuffData> buffList;

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

    public void OnBuffGroupRemove(Unit source, Unit target)
    {
        if (buffList.Count > 0)
        {
            foreach (var v in buffList)
            {
                Remove(in v, source, target);
            }
        }
    }

    void Remove(in BaseBuffData v,Unit source,Unit target)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(v.GetBuffIdType());
        IBuffRemoveHanlder buffRemoveHanlder = baseBuffHandler as IBuffRemoveHanlder;
        if (buffRemoveHanlder != null)
        {
            BuffHandlerVar buffHandlerVar = new BuffHandlerVar();
            buffHandlerVar.bufferValues = new Dictionary<Type, IBufferValue>(1);
            buffHandlerVar.bufferValues[typeof(BufferValue_TargetUnits)] = new BufferValue_TargetUnits() { targets = new Unit[1] { target } };
            buffHandlerVar.source = source;
            buffHandlerVar.playSpeed = 1;// 这个应该从角色属性计算得出,不过这里就先恒定为1好了.
            buffHandlerVar.data = v;
            buffRemoveHanlder.Remove(buffHandlerVar);
        }
    }
}

