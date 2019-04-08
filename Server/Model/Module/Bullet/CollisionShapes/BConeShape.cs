using System;
using UnityEngine;
using System.Collections;
using BulletSharp;
using ETModel;

namespace BulletUnity
{

    public class BConeShape : BCollisionShape
    {

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


        protected float height = 2f;
        public float Height
        {
            get { return height; }
            set
            {
                if (collisionShapePtr != null && value != height)
                {
                    Log.Error("Cannot change the height after the bullet shape has been created. Height is only the initial value " +
                                    "Use LocalScaling to change the shape of a bullet shape.");
                }
                else {
                    height = value;
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
                    ((ConeShape)collisionShapePtr).LocalScaling = value.ToBullet();
                }
            }
        }


        public override CollisionShape CopyCollisionShape()
        {
            ConeShape cs = new ConeShape(radius, height);
            cs.LocalScaling = m_localScaling.ToBullet();
            return cs;
        }

        public override CollisionShape GetCollisionShape()
        {
            if (collisionShapePtr == null)
            {
                collisionShapePtr = new ConeShape(radius, height);
                ((ConeShape)collisionShapePtr).LocalScaling = m_localScaling.ToBullet();
            }
            return collisionShapePtr;
        }
    }
}