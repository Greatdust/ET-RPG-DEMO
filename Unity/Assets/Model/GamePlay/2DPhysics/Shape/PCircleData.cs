using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    //可序列化的导出数据
    [Serializable]
    public class PCircleData : PBaseData
    {
        public float halfHeight;
        public float radius;
    }
}
