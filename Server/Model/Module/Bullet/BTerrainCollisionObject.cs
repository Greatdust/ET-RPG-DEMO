using UnityEngine;
using System;
using System.Collections;
using BulletSharp;
using ETModel;

namespace BulletUnity
{
    /* 
    Custom verson of the collision object for handling heightfields to deal with some issues matching terrains to heighfields
    1) Unity heitfiels have pivot at corner. Bullet heightfields have pivot at center
    2) Can't rotate unity heightfields        
    */
    public class BTerrainCollisionObject : BCollisionObject
    {
        public TerrainConfig terrainConfig;

        //called by Physics World just before rigid body is added to world.
        //the current rigid body properties are used to rebuild the rigid body.
        internal override bool _BuildCollisionObject()
        {
            

            BPhysicsWorld world = BPhysicsWorld.Get();
            if (m_collisionObject != null)
            {
                if (isInWorld && world != null)
                {
                    isInWorld = false;
                    world.RemoveCollisionObject(this);
                }
            }

            CollisionShape cs = m_collisionShape.GetCollisionShape();
            //rigidbody is dynamic if and only if mass is non zero, otherwise static

            if (m_collisionObject == null)
            {
                m_collisionObject = new CollisionObject();
                m_collisionObject.CollisionShape = cs;
                m_collisionObject.UserObject = this;

                BulletSharp.Math.Matrix worldTrans = BulletSharp.Math.Matrix.Identity;
                Vector3 pos = GetParent<Unit>().Position + new Vector3((float)terrainConfig.scale_x * .5f, (float)terrainConfig.scale_y * .5f, (float)terrainConfig.scale_z * .5f);
                worldTrans.Origin = pos.ToBullet();
                m_collisionObject.WorldTransform = worldTrans;
                m_collisionObject.CollisionFlags = m_collisionFlags;
            }
            else {
                m_collisionObject.CollisionShape = cs;
                BulletSharp.Math.Matrix worldTrans = BulletSharp.Math.Matrix.Identity;
                Vector3 pos = GetParent<Unit>().Position + new Vector3((float)terrainConfig.scale_x * .5f, (float)terrainConfig.scale_y * .5f, (float)terrainConfig.scale_z * .5f);
                worldTrans.Origin = pos.ToBullet();
                m_collisionObject.WorldTransform = worldTrans;
                m_collisionObject.CollisionFlags = m_collisionFlags;
            }
            return true;
        }

        public override void SetPositionAndRotation(Vector3 position, Quaternion rotation)
        {
            if (isInWorld)
            {
                BulletSharp.Math.Matrix newTrans = m_collisionObject.WorldTransform;
                newTrans.Origin = GetParent<Unit>().Position.ToBullet();
                m_collisionObject.WorldTransform = newTrans;
                GetParent<Unit>().Position = position;
                GetParent<Unit>().Rotation = rotation;
            } else
            {
                GetParent<Unit>().Position = position;
                GetParent<Unit>().Rotation = rotation;
            }
        }

    }
}
