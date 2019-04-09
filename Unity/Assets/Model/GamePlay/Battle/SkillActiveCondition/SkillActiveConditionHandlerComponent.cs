using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

[ObjectSystem]
public class SkillActiveConditionHandlerComponentAwakeSystem : AwakeSystem<SkillActiveConditionHandlerComponent>
{
    public override void Awake(SkillActiveConditionHandlerComponent self)
    {
        self.Awake();
    }
}

public class SkillActiveConditionHandlerComponent : ETModel.Component
{
    public static SkillActiveConditionHandlerComponent Instance;

    private Dictionary<string, BaseSkillData.IActiveConditionHandler> BuffHandlerDic;

    public void Awake()
    {
        Instance = this;
        BuffHandlerDic = new Dictionary<string, BaseSkillData.IActiveConditionHandler>();
        Load();
    }
    public void Load()
    {
        //List<Type> types = Game.EventSystem.GetTypes(typeof(BuffTypeAttribute));
        //foreach (Type type in types)
        //{
        //    object[] attrs = type.GetCustomAttributes(typeof(BuffTypeAttribute), false);

        //    foreach (object attr in attrs)
        //    {
        //        BuffTypeAttribute buffTypeAttr = (BuffTypeAttribute)attr;
        //        BaseBuffHandler obj = (BaseBuffHandler)Activator.CreateInstance(type);
        //        if (!this.BuffHandlerDic.ContainsKey(buffTypeAttr.BuffType))
        //        {
        //            this.BuffHandlerDic.Add(buffTypeAttr.BuffType, obj);
        //        }
        //    }
        //}
    }

    public BaseSkillData.IActiveConditionHandler GetHandler(string buffIdType)
    {
        BaseSkillData.IActiveConditionHandler handler;
        BuffHandlerDic.TryGetValue(buffIdType, out handler);
        return handler;
    }
    public override void Dispose()
    {
        base.Dispose();
        Instance = null;
    }
}

