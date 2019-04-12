using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Box2DSharp;
using Box2DSharp.Collision;
using System.Numerics;
using Box2DSharp.Common;

[BuffType(BuffIdType.RangeDetection)]
public class BuffHandler_RangeDetection : BaseBuffHandler, IBuffActionWithSetOutputHandler
{

    public IBufferValue[] ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        //主要目的是返回多个群体目标对象
        BufferValue_TargetUnits bufferValue_TargetUnits = new BufferValue_TargetUnits();

        Buff_RangeDetection buff_RangeDetection = (Buff_RangeDetection)buffHandlerVar.data;

        //根据传入进来的方向和位置计算做范围检测的区域

        BufferValue_Pos bufferValue_Pos = (BufferValue_Pos)buffHandlerVar.bufferValues[typeof(BufferValue_Pos)];

        Vector2 pos = new Vector2(bufferValue_Pos.aimPos.x, bufferValue_Pos.aimPos.z);

        BufferValue_Dir bufferValue_Dir = (BufferValue_Dir)buffHandlerVar.bufferValues[typeof(BufferValue_Dir)];

        UnityEngine.Vector2 angleV2 = new UnityEngine.Vector2(bufferValue_Dir.dir.x, bufferValue_Dir.dir.z);

        float angle = 0;
        if (angleV2.x > 0)
        {
            angle = UnityEngine.Vector2.Angle(UnityEngine.Vector2.up, angleV2) * Settings.Pi / 180;
        }
        var transform = new Transform(in pos, angle);
        PolyshapeQueryCallback polyshapeQueryCallback = new PolyshapeQueryCallback();
        AABB ab = new AABB();

        switch (buff_RangeDetection.shapeType)
        {
            case Buff_RangeDetection.CollisionShape.Box:
                // 初始的默认0方向是Vector3.forward. BOX2D中的角度是逆时针旋转的(0->π).
                var Vertices = new Vector2[4];

                float hx = buff_RangeDetection.shapeValue.x;
                float hy = buff_RangeDetection.shapeValue.y;
                Vertices[0].Set(-hx, -hy);
                Vertices[1].Set(hx, -hy);
                Vertices[2].Set(hx, hy);
                Vertices[3].Set(-hx, hy);
                // Transform vertices and normals.
                for (var i = 0; i < 4; ++i)
                {
                    Vertices[i] = MathUtils.Mul(transform, Vertices[i]);
                }              
                ab.UpperBound = ab.LowerBound = Vertices[0];
                for (var i = 1; i < 4; ++i)
                {
                    var v = Vertices[i];
                    ab.LowerBound = Vector2.Min(ab.LowerBound, v);
                    ab.UpperBound = Vector2.Max(ab.UpperBound, v);
                }
                var r = new Vector2(Settings.PolygonRadius, Settings.PolygonRadius);
                ab.LowerBound -= r;
                ab.UpperBound += r;
                break;
            case Buff_RangeDetection.CollisionShape.Circle:

                var p = transform.Position + MathUtils.Mul(transform.Rotation, transform.Position);
                float raidus = buff_RangeDetection.shapeValue.x;
                ab.LowerBound.Set(p.X - raidus, p.Y - raidus);
                ab.UpperBound.Set(p.X + raidus, p.Y + raidus);

                break;
        }
        PhysicWorldComponent.Instance.world.QueryAABB(polyshapeQueryCallback, ab);

        //拿到了所有检测到的Unit
        bufferValue_TargetUnits.targets = polyshapeQueryCallback.units.ToArray();
        
        Log.Debug(polyshapeQueryCallback.units.ListToString());
        return new IBufferValue[] { bufferValue_TargetUnits };
    }
}



