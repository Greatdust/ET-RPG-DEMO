using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

public abstract class BaseBuffHandler
{

}


public struct BuffHandlerVar
{
    public BaseBuffData data; // 对应的Buff数据
    public Unit source; // 来源方
    public float playSpeed;// 技能,特效等的播放速度
    public string skillId;
    public int skillLevel;//技能等级,用以处理一些数值需要跟随技能等级变动的情况
    public CancellationToken cancelToken;
    public Dictionary<Type,IBufferValue> bufferValues; // 处理Buff的时候所需要的参数

    public bool GetBufferValue<T>(out T value) where T : IBufferValue
    {
        if (bufferValues == null || !bufferValues.ContainsKey(typeof(T)))
        {
            value = default(T);
            return false;
        }
        value = (T)bufferValues[typeof(T)];
        return true;
    }

    //缓存BuffHandler执行过程中产生的中间数据,key的long是UnitId,string是buffSignal
    public readonly static Dictionary<(long, string), object> cacheDatas_object = new Dictionary<(long, string), object>();

    //避免装箱拆箱
    public readonly static Dictionary<(long, string), float> cacheDatas_float = new Dictionary<(long, string), float>();
    public readonly static Dictionary<(long, string), int> cacheDatas_int = new Dictionary<(long, string), int>();


}


public interface IBuffActionWithGetInputHandler
{
    void ActionHandle(BuffHandlerVar buffHandlerVar);
}

public interface IBuffActionWithSetOutputHandler
{
    IBufferValue[] ActionHandle(BuffHandlerVar buffHandlerVar);
}

public interface IBuffRemoveHanlder 
{
    void Remove(BuffHandlerVar buffHandlerVar);//正常的效果移除之类,会调用该方法
}

public interface IBuffUpdateHanlder
{
    void Update(BuffHandlerVar buffHandlerVar);//一些buff的效果更新/刷新. 用这个
}
