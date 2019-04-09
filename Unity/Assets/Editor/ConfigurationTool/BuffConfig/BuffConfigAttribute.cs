using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BuffConfigAttribute : BaseAttribute
{
    public string key;
    public Type buffType;
    public BuffConfigAttribute(string key,Type buffType)
    {
        this.key = key;
        this.buffType = buffType;
    }
}

public class BuffNonTargetAttribute : BaseAttribute
{

}

public class BuffHaveCollisionEventAttribute : BaseAttribute
{

}

public class EditorBuffActiveConditionAttribute : BaseAttribute
{
    public string key;
    public Type buffType;
    public EditorBuffActiveConditionAttribute(string key, Type buffType)
    {
        this.key = key;
        this.buffType = buffType;
    }
}