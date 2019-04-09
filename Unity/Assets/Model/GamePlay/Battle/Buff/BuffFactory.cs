using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;

public static class BuffFactory
{
    private static Dictionary<Type, Queue<BaseBuffData>> dic = new Dictionary<Type, Queue<BaseBuffData>>();

    public static BaseBuffData CreateBuff(Type type)
    {
        if (dic.TryGetValue(type, out var queue))
        {
            if (queue != null && queue.Count > 0)
            {
                return queue.Dequeue();
            }

        }

        return Activator.CreateInstance(type) as BaseBuffData;

    }

    public static void Recycle(BaseBuffData baseBuffData)
    {
        Type type = baseBuffData.GetType();
        if (dic.TryGetValue(type, out var queue))
        {
            if (queue != null)
            {
                if (queue.Count > 100)
                    return;
                 queue.Enqueue(baseBuffData);
                return;
            }
        }
        dic[type] = new Queue<BaseBuffData>();
        dic[type].Enqueue(baseBuffData);

    }
}

