using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public static class NetworkDataConvert
    {
        public static Vector3Info ToV3Info(this Vector3 vector3)
        {
            return new Vector3Info()
            {
                X = vector3.x,
                Y = vector3.y,
                Z = vector3.z
            };
        }
        public static Vector3 ToV3(this Vector3Info vector3)
        {
            return new Vector3()
            {
                x = vector3.X,
                y = vector3.Y,
                z = vector3.Z
            };
        }
    }
}
