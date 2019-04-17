using Box2DSharp.Collision.Shapes;
using Box2DSharp.Dynamics;
using ETModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{

    public class PStaticBodyMgrComponent : Component
    {
        public List<Unit> units;

        public const string configName = "MapData";

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            units = null;
        }
    }
}
