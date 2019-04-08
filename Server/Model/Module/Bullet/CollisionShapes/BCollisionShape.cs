using UnityEngine;
using System;
using System.Collections;
using BulletSharp.Math;
using BulletSharp;
using ETModel;

namespace BulletUnity {

    public abstract class BCollisionShape :Component, IDisposable {
        public enum CollisionShapeType {
            // dynamic
            BoxShape = 0,
            SphereShape = 1,
            CapsuleShape = 2,
            CylinderShape = 3,
            ConeShape = 4,
            ConvexHull = 5,
            CompoundShape = 6,

            // static
            BvhTriangleMeshShape = 7,
            StaticPlaneShape = 8,
        };

        protected CollisionShape collisionShapePtr = null;
        public bool drawGizmo = true;

        void OnDestroy() {
            Dispose(false);
        }

        public override void Dispose() {
            if (IsDisposed)
                return;
            base.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool isdisposing) {
            if (collisionShapePtr != null) {
                collisionShapePtr.Dispose();
                collisionShapePtr = null;
            }
        }

        public abstract CollisionShape CopyCollisionShape();

        public abstract CollisionShape GetCollisionShape();
    }
}


