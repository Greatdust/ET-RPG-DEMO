using System;
using UnityEngine;
using System.Collections;
using BulletSharp;
using System.Runtime.InteropServices;

namespace BulletUnity {
    public class BHeightfieldTerrainShape : BCollisionShape {

        public int upIndex;
        GCHandle pinnedTerrainData;
        PhyScalarType scalarType = PhyScalarType.Float;

        public HeightfieldTerrainShape hs;

        HeightfieldTerrainShape _CreateTerrainShape()
        {
            return hs;
        }

        public override CollisionShape CopyCollisionShape()
        {
            return _CreateTerrainShape();
        }

        public override CollisionShape GetCollisionShape() {
            if (collisionShapePtr == null) {
                collisionShapePtr = _CreateTerrainShape();
            }
            return collisionShapePtr;
        }

        protected override void Dispose(bool isdisposing)
        {
            if (collisionShapePtr != null)
            {
                collisionShapePtr.Dispose();
                collisionShapePtr = null;
            }
            pinnedTerrainData.Free();
        }
    }
}
