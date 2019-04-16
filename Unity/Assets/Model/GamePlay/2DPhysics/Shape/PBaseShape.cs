using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public abstract class PBaseShape
    {
        public UnitData unitData;

        public BodyType bodyType;

        public bool isSensor;
        /// <summary>
        /// 注意,Unity3D空间中X轴向右,但是Box2D中X轴向左
        /// 所以这个值在读取的时候,应该要确保读取到的是符号相反的值
        /// </summary>
        public float eulerAnglesY;

        public Vector3 offset;//位置偏移量,x和z用以直接影响2D平面的碰撞计算,y用以计算模拟的3D碰撞

    }
}
