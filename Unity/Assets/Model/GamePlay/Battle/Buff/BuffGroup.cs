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

    [HideInEditorMode]
    [NonSerialized]
    public long sourceUnitId;// 添加到一个Unit的BuffMgr上时,这个用以记录这个buffGroup的来源
    [InfoBox("该值对应buff配置表里的某个元素")]
    [LabelText("Buff类型Id")]
    [LabelWidth(150)]
    public int buffTypeId;   //从buff配置表中读取Buff应该显示出来的名字/描述等信息
    [InfoBox("-1代表持续到BUFF组被解除,0代表瞬间完成.大于0代表持续一段时间")]
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

    public void OnBuffUpdate(Unit source, Unit target,BaseBuffData baseBuffData)
    {
        BaseBuffHandler baseBuffHandler = BuffHandlerComponent.Instance.GetHandler(baseBuffData.GetBuffIdType());
        IBuffUpdateHanlder buffUpdateHandler = baseBuffHandler as IBuffUpdateHanlder;
        if (buffUpdateHandler != null)
        {
            BuffHandlerVar buffHandlerVar = new BuffHandlerVar();
            buffHandlerVar.bufferValues = new Dictionary<Type, IBufferValue>(1);
            buffHandlerVar.bufferValues[typeof(BufferValue_TargetUnits)] = new BufferValue_TargetUnits() { targets = new Unit[1] { target } };
            buffHandlerVar.source = source;
            buffHandlerVar.playSpeed = 1;// 这个应该从角色属性计算得出,不过这里就先恒定为1好了.
            buffHandlerVar.data = baseBuffData;
            buffUpdateHandler.Update(buffHandlerVar);
        }
    }
}

