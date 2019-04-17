using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    //可序列化的导出数据
    [Serializable]
    public abstract class PBaseData
    {
        public Vector3Serializer pos;
        public Vector3Serializer offset;
        public float eulerAnglesY;
        public BodyType bodyType;
    }
}
