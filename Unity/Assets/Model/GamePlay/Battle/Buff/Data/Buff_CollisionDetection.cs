using BulletSharp;
using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using static BulletUnity.BCollisionShape;

[Serializable]
public struct CollisionShape
{
    //决定下面的参数组合成哪个形状
    public CollisionShapeType collisionShapeType;
    public Vector3 extent;//bullet 引擎的参数
                          //如果是Cone,Cylinder,Sphere之类的. 那么x代表第一个参数,y代表第二次参数
                          //例如Cone(圆锥),x代表半径,y代表高度
                          //例如Cylinder(圆柱体),x代表半径,y代表高度
                          //例如Sphere,x直接就代表半径了

    public Vector3 offset; // 这是这个碰撞体,相对这个检测中所有碰撞体的原点位置的偏移距离

}

[Serializable]
public struct Buff_CollisionDetection : IBuffData
{
    //这里只做基础类型的检测. 其他的需要单独抽象一个服务器/客户端都可用的数据结构,以便还原出对应的碰撞体
    //比如Mesh类型的,就记录一堆顶点.但是DEMO这里不做这个处理.相信大多数需要联网的游戏也不会用.
    //一般BOX,圆柱,球之间的无限组合,已经足够了. 毕竟就连CSGO那种游戏都是无数个BOX
    //其他的复合类型有需要的自己添加吧

    //碰撞体列表,为了处理可能的复合碰撞体
    public List<CollisionShape> collisionShapes ;

    [NonSerialized]
    public Vector3 position;//以哪个点为中心
    [NonSerialized]
    public Quaternion ratation;//碰撞体的角度

    //在可以战斗的场景中,应该要提前创建好一个整合的碰撞体
    [NonSerialized]
    public CompoundShape compoundShape;



    public string GetBuffIdType()
    {
        return BuffIdType.CollisionDetection;
    }
}
