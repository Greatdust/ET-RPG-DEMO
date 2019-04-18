using ETModel;
using System;
using System.Collections.Generic;
using System.Text;


public static class BuffGroupSystem
{
    public static void AddBuff(this BuffGroup self, BaseBuffData baseBuffData)
    {
        self.buffList.Add(baseBuffData);
    }

    public static int IsExist(this BuffGroup self, string buffId)
    {
        int num = 0;
        foreach (var v in self.buffList)
        {
            if (v.GetBuffIdType() == buffId)
            {
                num++;
            }
        }
        return num;
    }

    public static void OnBuffGroupRemove(this BuffGroup self, Unit source, Unit target)
    {
        if (self.buffList.Count > 0)
        {
            foreach (var v in self.buffList)
            {
                self.Remove(v, source, target);
            }
        }
    }

    static void Remove(this BuffGroup self, BaseBuffData v, Unit source, Unit target)
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

