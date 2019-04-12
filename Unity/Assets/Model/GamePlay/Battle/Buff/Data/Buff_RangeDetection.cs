using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


//2D物理

[Serializable]
public struct Buff_RangeDetection : IBuffData
{
    public enum CollisionShape
    {
        Box,
        Circle
    }

    //DEMO中暂时就做矩形和圆形. 
    public CollisionShape shapeType;

    //如果是Box,x和y就是两条边,注意角色旋转的时候,shapevalue也会跟着旋转
    //如果是circle,x就是半径
    public Vector2 shapeValue ;



    [NonSerialized]
    public Vector2 position;//位置

    [NonSerialized]
    public float angle; //0或者2π代表上方(0,1),π代表(0,-1)


    public string GetBuffIdType()
    {
        return BuffIdType.RangeDetection; //范围检测
    }
}
