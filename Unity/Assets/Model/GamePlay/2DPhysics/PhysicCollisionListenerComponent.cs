using Box2DSharp.Collision.Collider;
using Box2DSharp.Dynamics;
using Box2DSharp.Dynamics.Contacts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            a.OnCollisionEnter(b);
            b.OnCollisionEnter(a);
            collisionStateRecorder[(a, b)] = true;
        }

        public void EndContact(Contact contact)
        {
            Unit a = contact.FixtureA.UserData as Unit;
            Unit b = contact.FixtureB.UserData as Unit;
            a.OnCollisionExit(b);
            b.OnCollisionExit(a);
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
