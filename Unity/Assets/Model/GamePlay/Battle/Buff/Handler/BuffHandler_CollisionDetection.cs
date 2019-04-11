using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.CollisionDetection)]
public class BuffHandler_CollisionDetection : BaseBuffHandler, IBuffActionWithSetOutputHandler
{
    public List<IBufferValue> ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        throw new NotImplementedException();
    }
}



