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
public class Buff_RangeDetection : BaseBuffData
{
    public enum CollisionShape
    {
        Box,
        Circle
    }

    //DEMO中暂时就做矩形和圆形. 
    public CollisionShape shapeType;

    //如果是Box,x和y就是两条边,默认状态下
    //如果是circle,x就是半径
    public Vector2 shapeValue ;

    public override string GetBuffIdType()
    {
        return BuffIdType.RangeDetection; //范围检测
    }
}
