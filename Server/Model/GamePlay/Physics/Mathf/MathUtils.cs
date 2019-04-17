using Box2DSharp.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public static class BOX2DMathfUtils
    {
        public static Vector2  GetVector2(this UnityEngine.Vector2 vector)
        {
            return new Vector2(vector.x, vector.y);
        }
        public static Vector2 ToVector2(this UnityEngine.Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }
        public static UnityEngine.Vector3 ToVector3(this Vector2 vector, float Y)
        {
            return new UnityEngine.Vector3(vector.X, Y, vector.Y);
        }

        //只变化Y轴的角度
        public static Rotation ToRotation2D(this UnityEngine.Quaternion quaternion)
        {
            float angle = ToAngle(UnityEngine.Quaternion.QuaternionToEuler(quaternion).y);
            return new Rotation(angle);

        }

        public static float ToAngle(float degreeInU3D)
        {
            float angle = -degreeInU3D * Settings.Pi / 180;
            return angle;
        }

        //只变化Y轴的角度
        public static Rotation ToRotation2D(this UnityEngine.Vector3 dir)
        {
            UnityEngine.Quaternion quaternion = UnityEngine.Quaternion.LookRotation(dir, UnityEngine.Vector3.up);
            return ToRotation2D(quaternion);
        }


        //只变化Y轴的角度
        public static UnityEngine.Quaternion ToRotation3D(this Rotation rot)
        {
            return UnityEngine.Quaternion.Euler(0, -rot.Angle * 180 / Settings.Pi, 0);
        }
    }
}
