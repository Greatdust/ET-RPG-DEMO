using System;
using UnityEngine;
using System.Collections;
using BulletSharp;
using BulletSharp.SoftBody;
using System.Collections.Generic;
using ETModel;

namespace BulletUnity
{
    [ObjectSystem]
    public class BPhysicsWorldAwakeSystem : AwakeSystem<BPhysicsWorld>
    {
        public override void Awake(BPhysicsWorld self)
        {
            self.Awake();
        }
    }

    public class BPhysicsWorld : Component, IDisposable
    {

        public enum WorldType
        {
            CollisionOnly,
            RigidBodyDynamics,
            MultiBodyWorld, //for FeatherStone forward dynamics I think
            SoftBodyAndRigidBody,
        }

        public enum CollisionConfType
        {
            DefaultDynamicsWorldCollisionConf,
            SoftBodyRigidBodyCollisionConf,
        }

        public enum BroadphaseType
        {
            DynamicAABBBroadphase,
            Axis3SweepBroadphase,
            Axis3SweepBroadphase_32bit,
            SimpleBroadphase,
        }

        protected static BPhysicsWorld singleton;
        protected static bool _isDisposed = false;
        const int axis3SweepMaxProxies = 32766;

        public static BPhysicsWorld Get()
        {
            return singleton;
        }



        WorldType m_worldType = WorldType.RigidBodyDynamics;
        public WorldType worldType
        {
            get { return m_worldType; }
            set
            {
                if (value != m_worldType && m_world != null)
                {
                    Log.Error("Can't modify a Physics World after simulation has started");
                    return;
                }
                m_worldType = value;
            }
        }


        CollisionConfType m_collisionType = CollisionConfType.DefaultDynamicsWorldCollisionConf;
        public CollisionConfType collisionType
        {
            get { return m_collisionType; }
            set
            {
                if (value != m_collisionType && m_world != null)
                {
                    Log.Error("Can't modify a Physics World after simulation has started");
                    return;
                }
                m_collisionType = value;
            }
        }


        BroadphaseType m_broadphaseType = BroadphaseType.DynamicAABBBroadphase;
        public BroadphaseType broadphaseType
        {
            get { return m_broadphaseType; }
            set
            {
                if (value != m_broadphaseType && m_world != null)
                {
                    Log.Error("Can't modify a Physics World after simulation has started");
                    return;
                }
                m_broadphaseType = value;
            }
        }


        Vector3 m_axis3SweepBroadphaseMin = new Vector3(-1000f, -1000f, -1000f);
        public Vector3 axis3SweepBroadphaseMin
        {
            get { return m_axis3SweepBroadphaseMin; }
            set
            {
                if (value != m_axis3SweepBroadphaseMin && m_world != null)
                {
                    Log.Error("Can't modify a Physics World after simulation has started");
                    return;
                }
                m_axis3SweepBroadphaseMin = value;
            }
        }


        Vector3 m_axis3SweepBroadphaseMax = new Vector3(1000f, 1000f, 1000f);
        public Vector3 axis3SweepBroadphaseMax
        {
            get { return m_axis3SweepBroadphaseMax; }
            set
            {
                if (value != m_axis3SweepBroadphaseMax && m_world != null)
                {
                    Log.Error("Can't modify a Physics World after simulation has started");
                    return;
                }
                m_axis3SweepBroadphaseMax = value;
            }
        }


        Vector3 m_gravity = new Vector3(0f, -9.8f, 0f);
        public Vector3 gravity
        {
            get { return m_gravity; }
            set
            {
                if (_ddWorld != null)
                {
                    BulletSharp.Math.Vector3 grav = value.ToBullet();
                    _ddWorld.SetGravity(ref grav);
                }
                m_gravity = value;
            }
        }

        float m_fixedTimeStep = 1f / 60f;
        public float fixedTimeStep
        {
            get
            {
                return m_fixedTimeStep;
            }
            set
            {
                if (lateUpdateHelper != null)
                {
                    lateUpdateHelper.m_fixedTimeStep = (long)(value * 1000);
                }
                m_fixedTimeStep = value;
            }
        }


        /*
        [SerializeField]
        bool m_doCollisionCallbacks = true;
        public bool doCollisionCallbacks
        {
            get { return m_doCollisionCallbacks; }
            set { m_doCollisionCallbacks = value; }
        }
        */

        BPhysicsWorldLateHelper lateUpdateHelper;

        CollisionConfiguration CollisionConf;
        CollisionDispatcher Dispatcher;
        BroadphaseInterface Broadphase;
        SoftBodyWorldInfo softBodyWorldInfo;
        ConstraintSolver constraintSolver;
        GhostPairCallback ghostPairCallback = null;
        ulong sequentialImpulseConstraintSolverRandomSeed = 12345;



        CollisionWorld m_world;
        public CollisionWorld world
        {
            get { return m_world; }
            set { m_world = value; }
        }

        private DiscreteDynamicsWorld _ddWorld; // convenience variable so we arn't typecasting all the time.

        public int frameCount
        {
            get
            {
                if (lateUpdateHelper != null)
                {
                    return lateUpdateHelper.m__frameCount;
                }
                else
                {
                    return -1;
                }
            }
        }

        public float timeStr;

        public void RegisterCollisionCallbackListener(BCollisionObject.BICollisionCallbackEventHandler toBeAdded)
        {
            if (lateUpdateHelper != null) lateUpdateHelper.RegisterCollisionCallbackListener(toBeAdded);
        }

        public void DeregisterCollisionCallbackListener(BCollisionObject.BICollisionCallbackEventHandler toBeRemoved)
        {
            if (lateUpdateHelper != null) lateUpdateHelper.DeregisterCollisionCallbackListener(toBeRemoved);
        }



        //It is critical that Awake be called before any other scripts call BPhysicsWorld.Get()
        //Set this script and any derived classes very early in script execution order.
        public virtual void Awake()
        {
            _isDisposed = false;
            singleton = this;
            singleton._InitializePhysicsWorld();
        }

        protected virtual void OnDestroy()
        {

            Dispose(false);
        }


        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public bool AddAction(IAction action)
        {
            if (!_isDisposed)
            {
                if (m_worldType < WorldType.RigidBodyDynamics)
                {
                    Log.Error("World type must not be collision only");
                }
                else
                {
                    ((DynamicsWorld)world).AddAction(action);
                }
                return true;
            }
            return false;
        }

        public void RemoveAction(IAction action)
        {
            if (!_isDisposed)
            {
                if (m_worldType < WorldType.RigidBodyDynamics)
                {
                    Log.Error("World type must not be collision only");
                }
              ((DiscreteDynamicsWorld)m_world).RemoveAction(action);
            }
        }

        public bool AddCollisionObject(BCollisionObject co)
        {


            if (!_isDisposed)
            {

                if (co._BuildCollisionObject())
                {
                    m_world.AddCollisionObject(co.GetCollisionObject(), co.groupsIBelongTo, co.collisionMask);
                    co.isInWorld = true;
                    if (ghostPairCallback == null && co is BGhostObject && world is DynamicsWorld)
                    {
                        ghostPairCallback = new GhostPairCallback();
                        ((DynamicsWorld)world).PairCache.SetInternalGhostPairCallback(ghostPairCallback);
                    }
                    if (co is BCharacterController && world is DynamicsWorld)
                    {
                        AddAction(((BCharacterController)co).GetKinematicCharacterController());
                    }

                }
                return true;
            }
            return false;
        }

        public void RemoveCollisionObject(BCollisionObject co)
        {

            if (!_isDisposed)
            {
                if (co is BCharacterController && world is DynamicsWorld)
                {
                    RemoveAction(((BCharacterController)co).GetKinematicCharacterController());
                }

                m_world.RemoveCollisionObject(co.GetCollisionObject());
                co.isInWorld = false;
            }
        }





        public void RemoveSoftBody(BulletSharp.SoftBody.SoftBody softBody)
        {
            if (!_isDisposed && m_world is BulletSharp.SoftBody.SoftRigidDynamicsWorld)
            {

                ((BulletSharp.SoftBody.SoftRigidDynamicsWorld)m_world).RemoveSoftBody(softBody);
                if (softBody.UserObject is BCollisionObject) ((BCollisionObject)softBody.UserObject).isInWorld = false;
            }
        }

        protected virtual void _InitializePhysicsWorld()
        {
            _isDisposed = false;
            CreatePhysicsWorld(out m_world, out CollisionConf, out Dispatcher, out Broadphase, out constraintSolver, out softBodyWorldInfo);
            if (m_world is DiscreteDynamicsWorld)
            {
                _ddWorld = (DiscreteDynamicsWorld)m_world;
            }
            //Add a BPhysicsWorldLateHelper component to call FixedUpdate
            lateUpdateHelper = GetParent<Entity>().GetComponent<BPhysicsWorldLateHelper>();
            if (lateUpdateHelper == null)
            {
                lateUpdateHelper = GetParent<Entity>().AddComponent<BPhysicsWorldLateHelper>();
            }
            lateUpdateHelper.m_world = world;
            lateUpdateHelper.m_ddWorld = _ddWorld;
            lateUpdateHelper.m_physicsWorld = this;
            lateUpdateHelper.m__frameCount = 0;
        }

        /*
        Does not set any local variables. Is safe to use to create duplicate physics worlds for independant simulation.
        */
        public bool CreatePhysicsWorld(out CollisionWorld world,
                                        out CollisionConfiguration collisionConfig,
                                        out CollisionDispatcher dispatcher,
                                        out BroadphaseInterface broadphase,
                                        out ConstraintSolver solver,
                                        out SoftBodyWorldInfo softBodyWorldInfo)
        {
            bool success = true;
            if (m_worldType == WorldType.SoftBodyAndRigidBody && m_collisionType == CollisionConfType.DefaultDynamicsWorldCollisionConf)
            {

                m_collisionType = CollisionConfType.SoftBodyRigidBodyCollisionConf;
                success = false;
            }

            collisionConfig = null;
            if (m_collisionType == CollisionConfType.DefaultDynamicsWorldCollisionConf)
            {
                collisionConfig = new DefaultCollisionConfiguration();
            }
            else if (m_collisionType == CollisionConfType.SoftBodyRigidBodyCollisionConf)
            {
                collisionConfig = new SoftBodyRigidBodyCollisionConfiguration();
            }

            dispatcher = new CollisionDispatcher(collisionConfig);

            if (m_broadphaseType == BroadphaseType.DynamicAABBBroadphase)
            {
                broadphase = new DbvtBroadphase();
            }
            else if (m_broadphaseType == BroadphaseType.Axis3SweepBroadphase)
            {
                broadphase = new AxisSweep3(m_axis3SweepBroadphaseMin.ToBullet(), m_axis3SweepBroadphaseMax.ToBullet(), axis3SweepMaxProxies);
            }
            else if (m_broadphaseType == BroadphaseType.Axis3SweepBroadphase_32bit)
            {
                broadphase = new AxisSweep3_32Bit(m_axis3SweepBroadphaseMin.ToBullet(), m_axis3SweepBroadphaseMax.ToBullet(), axis3SweepMaxProxies);
            }
            else
            {
                broadphase = null;
            }
            world = null;
            softBodyWorldInfo = null;
            solver = null;
            if (m_worldType == WorldType.CollisionOnly)
            {
                world = new CollisionWorld(dispatcher, broadphase, collisionConfig);
            }
            else if (m_worldType == WorldType.RigidBodyDynamics)
            {
                world = new DiscreteDynamicsWorld(dispatcher, broadphase, null, collisionConfig);
            }
            else if (m_worldType == WorldType.MultiBodyWorld)
            {
                MultiBodyConstraintSolver mbConstraintSolver = new MultiBodyConstraintSolver();
                constraintSolver = mbConstraintSolver;
                world = new MultiBodyDynamicsWorld(dispatcher, broadphase, mbConstraintSolver, collisionConfig);
            }
            else if (m_worldType == WorldType.SoftBodyAndRigidBody)
            {
                SequentialImpulseConstraintSolver siConstraintSolver = new SequentialImpulseConstraintSolver();
                constraintSolver = siConstraintSolver;
                siConstraintSolver.RandSeed = sequentialImpulseConstraintSolverRandomSeed;
                m_world = new SoftRigidDynamicsWorld(Dispatcher, Broadphase, siConstraintSolver, CollisionConf);
                _ddWorld = (DiscreteDynamicsWorld)m_world;
                SoftRigidDynamicsWorld _sworld = (SoftRigidDynamicsWorld)m_world;

                m_world.DispatchInfo.EnableSpu = true;
                _sworld.WorldInfo.SparseSdf.Initialize();
                _sworld.WorldInfo.SparseSdf.Reset();
                _sworld.WorldInfo.AirDensity = 1.2f;
                _sworld.WorldInfo.WaterDensity = 0;
                _sworld.WorldInfo.WaterOffset = 0;
                _sworld.WorldInfo.WaterNormal = BulletSharp.Math.Vector3.Zero;
                _sworld.WorldInfo.Gravity = m_gravity.ToBullet();
            }
            if (world is DiscreteDynamicsWorld)
            {
                ((DiscreteDynamicsWorld)world).Gravity = m_gravity.ToBullet();
            }

            return success;
        }

        protected void Dispose(bool disposing)
        {

            if (lateUpdateHelper != null)
            {
                lateUpdateHelper.m_ddWorld = null;
                lateUpdateHelper.m_world = null;
            }
            if (m_world != null)
            {
                //remove/dispose constraints
                int i;
                if (_ddWorld != null)
                {

                    for (i = _ddWorld.NumConstraints - 1; i >= 0; i--)
                    {
                        TypedConstraint constraint = _ddWorld.GetConstraint(i);
                        _ddWorld.RemoveConstraint(constraint);
                        constraint.Dispose();
                    }
                }


                //remove the rigidbodies from the dynamics world and delete them
                for (i = m_world.NumCollisionObjects - 1; i >= 0; i--)
                {
                    CollisionObject obj = m_world.CollisionObjectArray[i];
                    RigidBody body = obj as RigidBody;
                    if (body != null && body.MotionState != null)
                    {

                        body.MotionState.Dispose();
                    }
                    m_world.RemoveCollisionObject(obj);
                    if (obj.UserObject is BCollisionObject) ((BCollisionObject)obj.UserObject).isInWorld = false;

                    obj.Dispose();
                }

                if (m_world.DebugDrawer != null)
                {
                    if (m_world.DebugDrawer is IDisposable)
                    {
                        IDisposable dis = (IDisposable)m_world.DebugDrawer;
                        dis.Dispose();
                    }
                }

                m_world.Dispose();
                Broadphase.Dispose();
                Dispatcher.Dispose();
                CollisionConf.Dispose();
                _ddWorld = null;
                m_world = null;
            }

            if (Broadphase != null)
            {
                Broadphase.Dispose();
                Broadphase = null;
            }
            if (Dispatcher != null)
            {
                Dispatcher.Dispose();
                Dispatcher = null;
            }
            if (CollisionConf != null)
            {
                CollisionConf.Dispose();
                CollisionConf = null;
            }
            if (constraintSolver != null)
            {
                constraintSolver.Dispose();
                constraintSolver = null;
            }
            if (softBodyWorldInfo != null)
            {
                softBodyWorldInfo.Dispose();
                softBodyWorldInfo = null;
            }
            _isDisposed = true;
            singleton = null;
        }
    }

    public class BDefaultCollisionHandler
    {
        HashSet<BCollisionObject.BICollisionCallbackEventHandler> collisionCallbackListeners = new HashSet<BCollisionObject.BICollisionCallbackEventHandler>();

        public void RegisterCollisionCallbackListener(BCollisionObject.BICollisionCallbackEventHandler toBeAdded)
        {
            collisionCallbackListeners.Add(toBeAdded);
        }

        public void DeregisterCollisionCallbackListener(BCollisionObject.BICollisionCallbackEventHandler toBeRemoved)
        {
            collisionCallbackListeners.Remove(toBeRemoved);
        }

        public void OnPhysicsStep(CollisionWorld world)
        {
            Dispatcher dispatcher = world.Dispatcher;
            int numManifolds = dispatcher.NumManifolds;
            for (int i = 0; i < numManifolds; i++)
            {
                PersistentManifold contactManifold = dispatcher.GetManifoldByIndexInternal(i);
                CollisionObject a = contactManifold.Body0;
                CollisionObject b = contactManifold.Body1;
                if (a is CollisionObject && a.UserObject is BCollisionObject && ((BCollisionObject)a.UserObject).collisionCallbackEventHandler != null)
                {
                    ((BCollisionObject)a.UserObject).collisionCallbackEventHandler.OnVisitPersistentManifold(contactManifold);
                }
                if (b is CollisionObject && b.UserObject is BCollisionObject && ((BCollisionObject)b.UserObject).collisionCallbackEventHandler != null)
                {
                    ((BCollisionObject)b.UserObject).collisionCallbackEventHandler.OnVisitPersistentManifold(contactManifold);
                }
            }
            foreach (BCollisionObject.BICollisionCallbackEventHandler coeh in collisionCallbackListeners)
            {
                if (coeh != null) coeh.OnFinishedVisitingManifolds();
            }
        }
    }
}
