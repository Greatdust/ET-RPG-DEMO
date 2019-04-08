using System;
using UnityEngine;
using System.Collections;
using BulletSharp;
using ETModel;

namespace BulletUnity {

    public class BSphereShape : BCollisionShape {

        protected float radius = 1f;
        public float Radius
        {
            get { return radius; }
            set
            {
                if (collisionShapePtr != null && value != radius)
                {
                    Log.Error("Cannot change the radius after the bullet shape has been created. Radius is only the initial value " +
                                    "Use LocalScaling to change the shape of a bullet shape.");
                }
                else {
                    radius = value;
                }
            }
        }


        protected Vector3 m_localScaling = Vector3.one;
        public Vector3 LocalScaling
        {
            get { return m_localScaling; }
            set
            {
                m_localScaling = value;
                if (collisionShapePtr != null)
                {
                    ((SphereShape)collisionShapePtr).LocalScaling = value.ToBullet();
                }
            }
        }


        public override CollisionShape CopyCollisionShape()
        {
            SphereShape ss = new SphereShape(radius);
            ss.LocalScaling = m_localScaling.ToBullet();
            return ss;
        }

        public override CollisionShape GetCollisionShape() {
            if (collisionShapePtr == null) {
                collisionShapePtr = new SphereShape(radius);
                ((SphereShape)collisionShapePtr).LocalScaling = m_localScaling.ToBullet();
            }
            return collisionShapePtr;
        }
    }
}
