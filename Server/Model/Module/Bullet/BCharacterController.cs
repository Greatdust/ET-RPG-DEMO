using UnityEngine;
using System.Collections;
using BulletSharp;
using ETModel;

namespace BulletUnity
{
    [ObjectSystem]
    public class BCharacterControllerAwakeSystem : AwakeSystem<BCharacterController,BCollisionShape>
    {
        public override void Awake(BCharacterController self, BCollisionShape collisionShape)
        {
            self.Awake(collisionShape);
        }
    }

    [ObjectSystem]
    public class BCharacterControllerFixedUpdateSystem : FixedUpdateSystem<BCharacterController>
    {
        public override void FixedUpdate(BCharacterController self)
        {
            self.FixedUpdate();
        }
    }

    public class BCharacterController : BPairCachingGhostObject
    {

        KinematicCharacterController m_characterController;
        public float stepHeight = 1f;
        public int upAxis = 1; //0=x, 1=y, 2=z


        public KinematicCharacterController GetKinematicCharacterController()
        {
            return m_characterController;
        }

        public override void Awake(BCollisionShape collisionShape)
        {
            base.Awake(collisionShape);
            m_collisionFlags = BulletSharp.CollisionFlags.CharacterObject;
        }

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
            m_collisionShape.GetCollisionShape();
            if (m_collisionObject == null)
            {
                m_collisionObject = new PairCachingGhostObject();
                m_collisionObject.CollisionShape = m_collisionShape.GetCollisionShape();
                m_collisionObject.CollisionFlags = m_collisionFlags;
                m_characterController = new KinematicCharacterController((PairCachingGhostObject)m_collisionObject, (ConvexShape)m_collisionShape.GetCollisionShape(), stepHeight, upAxis);
                BulletSharp.Math.Matrix worldTrans;
                BulletSharp.Math.Quaternion q = GetParent<Unit>().Rotation.ToBullet();
                BulletSharp.Math.Matrix.RotationQuaternion(ref q, out worldTrans);
                worldTrans.Origin = GetParent<Unit>().Position.ToBullet();
                m_collisionObject.WorldTransform = worldTrans;
                m_collisionObject.UserObject = this;
                //world.world.AddCollisionObject(m_collisionObject, CollisionFilterGroups.CharacterFilter, CollisionFilterGroups.StaticFilter | CollisionFilterGroups.DefaultFilter);
                //((DynamicsWorld)world.world).AddAction(m_characterController);
            }
            else {
                m_collisionObject.CollisionShape = m_collisionShape.GetCollisionShape();
                m_collisionObject.CollisionFlags = m_collisionFlags;
                if (m_characterController != null)
                {
                    world.RemoveAction(m_characterController);
                }
                m_characterController = new KinematicCharacterController((PairCachingGhostObject)m_collisionObject, (ConvexShape)m_collisionShape.GetCollisionShape(), stepHeight, upAxis);
                BulletSharp.Math.Matrix worldTrans;
                BulletSharp.Math.Quaternion q = GetParent<Unit>().Rotation.ToBullet();
                BulletSharp.Math.Matrix.RotationQuaternion(ref q, out worldTrans);
                worldTrans.Origin = GetParent<Unit>().Position.ToBullet();
                m_collisionObject.WorldTransform = worldTrans;
                m_collisionObject.UserObject = this;
            }
            return true;
        }

        public void Move(Vector3 displacement)
        {
            m_characterController.SetWalkDirection(displacement.ToBullet());
        }

        public void Jump()
        {
            m_characterController.Jump();
        }

        public void Rotate(float turnAmount)
        {
            BulletSharp.Math.Matrix xform = m_collisionObject.WorldTransform;
            BulletSharp.Math.Matrix orn = xform;
            BulletSharp.Math.Vector3 pos = xform.Origin;
            orn.Row4 = new BulletSharp.Math.Vector4(0, 0, 0, 1);
            BulletSharp.Math.Vector3 upDir = new BulletSharp.Math.Vector3(xform.M21, xform.M22, xform.M23);
            orn *= BulletSharp.Math.Matrix.RotationAxis(upDir, turnAmount);
            orn.Row4 = new BulletSharp.Math.Vector4(pos.X, pos.Y, pos.Z, 1);
            m_collisionObject.WorldTransform = orn;
        }

        public void FixedUpdate()
        {
            BulletSharp.Math.Matrix trans;
            m_collisionObject.GetWorldTransform(out trans);
            GetParent<Unit>().Position = BSExtensionMethods2.ExtractTranslationFromMatrix(ref trans);
            GetParent<Unit>().Rotation = BSExtensionMethods2.ExtractRotationFromMatrix(ref trans);
        }
    }
}
