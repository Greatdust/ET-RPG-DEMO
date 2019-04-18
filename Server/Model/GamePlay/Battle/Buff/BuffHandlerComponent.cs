using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/// <summary>
/// 提供一个可供寻找到BUFF对应的Handler的组件
/// </summary>
public class BuffHandlerComponent : ETModel.Component
{
    public static BuffHandlerComponent Instance;

    public Dictionary<string, BaseBuffHandler> BuffHandlerDic;

    public override void Dispose()
    {
        base.Dispose();
        Instance = null;
        BuffHandlerDic.Clear();
    }
}

