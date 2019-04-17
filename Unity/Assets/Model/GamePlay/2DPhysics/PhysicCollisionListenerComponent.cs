using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class PhysicCollisionListenerAwakeSystem : AwakeSystem<PhysicCollisionListenerComponent>
    {
        public override void Awake(PhysicCollisionListenerComponent self)
        {
            Game.Scene.GetComponent<PhysicWorldComponent>().world.SetContactListener(self);
        }
    }

    [ObjectSystem]
    public class PhysicCollisionListenerFixedUpdateSystem : FixedUpdateSystem<PhysicCollisionListenerComponent>
    {
        public override void FixedUpdate(PhysicCollisionListenerComponent self)
        {
            self.FixedUpdate();
        }
    }

    //管理Unity的OnCollisionEnter/Stay/Exit方法
    public class PhysicCollisionListenerComponent : Component, IContactListener
    {
        public readonly Dictionary<(Unit, Unit), bool> collisionStateRecorder = new Dictionary<(Unit, Unit), bool>();

        public void BeginContact(Contact contact)
        {
            

            Unit a = contact.FixtureA.UserData as Unit;
            Unit b = contact.FixtureB.UserData as Unit;
            //Log.Debug("碰撞了,但是需要判断高度!");

            //Log.Debug("a的实际Y位置{0}" ,a.Position.y + a.OffsetY);
            //Log.Debug("b的实际Y位置{0}", b.Position.y + b.OffsetY);
            //Log.Debug("a和b的半高之和{0}", a.HalfHeight + b.HalfHeight);
            //模拟高度,判断双方在高度上没有碰撞
            
            if (Mathf.Abs(a.Position.y +a.OffsetY  - b.Position.y - b.OffsetY) - (a.HalfHeight + b.HalfHeight)  > 0)
            {
                collisionStateRecorder[(a, b)] = false;
                return;
            }
            RayCastAimUnitCallback rayCastAimUnitCallback = new RayCastAimUnitCallback(b);
            Game.Scene.GetComponent<PhysicWorldComponent>().world.RayCast(rayCastAimUnitCallback, a.Position.ToVector2(), b.Position.ToVector2());
            Vector3 hitPoint = rayCastAimUnitCallback.Point.ToVector3((a.Position.y + a.OffsetY + b.Position.y + b.OffsetY) / 2);

            //Log.Debug("确定碰撞!{0}  {1}",a.GameObject.name , b.GameObject.name);
            a.OnCollisionEnter(b, hitPoint);
            b.OnCollisionEnter(a, hitPoint);
            collisionStateRecorder[(a, b)] = true;
        }

        public void EndContact(Contact contact)
        {

            Unit a = contact.FixtureA.UserData as Unit;
            Unit b = contact.FixtureB.UserData as Unit;
            if (!collisionStateRecorder[(a, b)]) return;
            RayCastAimUnitCallback rayCastAimUnitCallback = new RayCastAimUnitCallback(b);
            Game.Scene.GetComponent<PhysicWorldComponent>().world.RayCast(rayCastAimUnitCallback, a.Position.ToVector2(), b.Position.ToVector2());
            Vector3 hitPoint = rayCastAimUnitCallback.Point.ToVector3((a.Position.y + a.OffsetY + b.Position.y + b.OffsetY) / 2);
            a.OnCollisionExit(b, hitPoint);
            b.OnCollisionExit(a, hitPoint);
            collisionStateRecorder[(a, b)] = false;
        }

        public void PostSolve(Contact contact, in ContactImpulse impulse)
        {
            
        }

        public void PreSolve(Contact contact, in Manifold oldManifold)
        {
            
        }

        public void FixedUpdate()
        {
            foreach (var v in collisionStateRecorder)
            {
                if (v.Value)
                {
                    if (!v.Key.Item1.IsDisposed && !v.Key.Item2.IsDisposed)
                    {
                        v.Key.Item1.OnCollisionStay(v.Key.Item2);
                        v.Key.Item2.OnCollisionStay(v.Key.Item1);
                    }
                }
            }
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            collisionStateRecorder.Clear();
        }
    }

}
