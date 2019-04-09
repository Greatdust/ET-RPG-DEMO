using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public abstract class BaseBuffHandler
{

}



public interface IBuffActionWithGetInputHandler
{
    void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues);
}

public interface IBuffActionWithSetOutputHandler
{
    IBuffReturnedValue ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues);
}

public interface IBuffActionWithCollision
{
    void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues, Action<long> action);
}

public interface IBuffRemoveHanlder 
{
    void Remove(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues);//主动中断/打断或者正常的效果移除之类,会调用该方法
}
