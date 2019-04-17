using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


//2D物理
[LabelText("2D范围检测")]
[LabelWidth(150)]
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
    public Vector2Serializer shapeValue ;

    public bool FindFriend;

    //一个高度检测值. 用以模拟3D的检测效果.
    [LabelText("检测区域半高")]
    [LabelWidth(100)]
    public float halfHeight;

    public override string GetBuffIdType()
    {
        return BuffIdType.RangeDetection; //范围检测
    }
}
