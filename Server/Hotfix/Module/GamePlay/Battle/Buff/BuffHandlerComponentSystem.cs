using ETModel;
using System;
using System.Collections.Generic;
using System.Text;


[ObjectSystem]
public class BuffHandlerComponentAwakeSystem : AwakeSystem<BuffHandlerComponent>
{
    public override void Awake(BuffHandlerComponent self)
    {
        self.Awake();
    }
}

public static class BuffHandlerComponentSystem
{

    public static void Awake(this BuffHandlerComponent self)
    {
        BuffHandlerComponent.Instance = self;
        self.BuffHandlerDic = new Dictionary<string, BaseBuffHandler>();
        self.Load();
    }
    public static void Load(this BuffHandlerComponent self)
    {
        foreach (Type type in typeof(BuffHandlerComponentSystem).Assembly.GetTypes())
        {
            object[] attrs = type.GetCustomAttributes(typeof(BuffTypeAttribute), false);

            foreach (object attr in attrs)
            {
                BuffTypeAttribute buffTypeAttr = (BuffTypeAttribute)attr;
                BaseBuffHandler obj = (BaseBuffHandler)Activator.CreateInstance(type);
                if (!self.BuffHandlerDic.ContainsKey(buffTypeAttr.BuffType))
                {
                    self.BuffHandlerDic.Add(buffTypeAttr.BuffType, obj);
                }
            }
        }
    }

    public static BaseBuffHandler GetHandler(this BuffHandlerComponent self, string buffIdType)
    {
        BaseBuffHandler handler;
        self.BuffHandlerDic.TryGetValue(buffIdType, out handler);
        return handler;
    }
}

