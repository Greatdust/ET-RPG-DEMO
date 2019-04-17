using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    //可序列化的导出数据
    [Serializable]
    public class PBoxData
    {
        public Vector3Serializer pos;
        public Vector3Serializer offset;
        public Vector3Serializer size;
        public float eulerAnglesY;
    }
}
