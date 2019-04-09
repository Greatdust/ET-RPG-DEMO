using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ObjectSystem]
public class BuffHandlerComponentAwakeSystem : AwakeSystem<BuffHandlerComponent>
{
    public override void Awake(BuffHandlerComponent self)
    {
        self.Awake();
    }
}

/// <summary>
/// 提供一个可供寻找到BUFF对应的Handler的组件
/// </summary>
public class BuffHandlerComponent : ETModel.Component
{
    public static BuffHandlerComponent Instance;

    private Dictionary<string, BaseBuffHandler> BuffHandlerDic;

    public void Awake()
    {
        Instance = this;
        BuffHandlerDic = new Dictionary<string, BaseBuffHandler>();
        Load();
    }
    public void Load()
    {
        List<Type> types = Game.EventSystem.GetTypes(typeof(BuffTypeAttribute));
        foreach (Type type in types)
        {
            object[] attrs = type.GetCustomAttributes(typeof(BuffTypeAttribute), false);

            foreach (object attr in attrs)
            {
                BuffTypeAttribute buffTypeAttr = (BuffTypeAttribute)attr;
                BaseBuffHandler obj = (BaseBuffHandler)Activator.CreateInstance(type);
                if (!this.BuffHandlerDic.ContainsKey(buffTypeAttr.BuffType))
                {
                    this.BuffHandlerDic.Add(buffTypeAttr.BuffType, obj);
                }
            }
        }
    }

    public BaseBuffHandler GetHandler(string buffIdType)
    {
        BaseBuffHandler handler;
        BuffHandlerDic.TryGetValue(buffIdType, out handler);
        return handler;
    }
    public override void Dispose()
    {
        base.Dispose();
        Instance = null;
    }
}

