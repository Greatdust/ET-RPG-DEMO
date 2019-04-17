using Box2DSharp.Dynamics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public class PolyshapeQueryCallback : IQueryCallback
    {
        public List<Unit> units = new List<Unit>();

        public bool QueryCallback(Fixture fixture)
        {
            Unit unit = fixture.UserData as Unit;
            if (unit != null && !unit.IsDisposed)
                units.Add(unit);
            return true;
        }
    }
    public class RayCastAimUnitCallback : IRayCastCallback
    {
        public bool Hit;

        public Unit aim;

        public Vector2 Normal;

        public Vector2 Point;

        public RayCastAimUnitCallback(Unit unit)
        {
            Hit = false; aim = unit;
        }

        public float RayCastCallback(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
        {
            var body = fixture.Body;
            if (fixture.UserData != aim)
            {
                return -1.0f;
            }

            Hit = true;
            Point = point;
            Normal = normal;

            // By returning the current fraction, we instruct the calling code to clip the ray and
            // continue the ray-cast to the next fixture. WARNING: do not assume that fixtures
            // are reported in order. However, by clipping, we can always get the closest fixture.
            return fraction;
        }
    }
    public class RayCastStaticObjCallback : IRayCastCallback
    {
        public bool Hit;

        public Vector2 Normal;

        public Vector2 Point;

        public float RayCastCallback(Fixture fixture, in Vector2 point, in Vector2 normal, float fraction)
        {
            var body = fixture.Body;

            if (fixture.Body.BodyType == BodyType.StaticBody)
            {
                Hit = true;
                Point = point;
                Normal = normal;
                return fraction;
            }

            // By returning the current fraction, we instruct the calling code to clip the ray and
            // continue the ray-cast to the next fixture. WARNING: do not assume that fixtures
            // are reported in order. However, by clipping, we can always get the closest fixture.
            return -1.0f;
        }
    }
}
