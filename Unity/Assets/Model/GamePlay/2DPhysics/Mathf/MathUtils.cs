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
    }
}
