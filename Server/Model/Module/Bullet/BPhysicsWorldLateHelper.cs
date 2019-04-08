using UnityEngine;
using BulletSharp;
using ETModel;

namespace BulletUnity
{
    [ObjectSystem]
    public class BPhysicsWorldLateHelperAwakeSystem : AwakeSystem<BPhysicsWorldLateHelper>
    {
        public override void Awake(BPhysicsWorldLateHelper self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class BPhysicsWorldLateHelperFixedUpdateSystem : FixedUpdateSystem<BPhysicsWorldLateHelper>
    {
        public override void FixedUpdate(BPhysicsWorldLateHelper self)
        {
            self.FixedUpdate();
        }
    }

    /**
    This script is last in the script execution order. Its purpose is to ensure that StepSimulation is called after other scripts LateUpdate calls
    Do not add this script manually. The BPhysicsWorld will add it.
    */
    public class BPhysicsWorldLateHelper : Component
    {
        internal BPhysicsWorld m_physicsWorld;
        internal BDefaultCollisionHandler m_collisionEventHandler = new BDefaultCollisionHandler();
        public void RegisterCollisionCallbackListener(BCollisionObject.BICollisionCallbackEventHandler toBeAdded)
        {
            if (m_collisionEventHandler != null) m_collisionEventHandler.RegisterCollisionCallbackListener(toBeAdded);
        }

        public void DeregisterCollisionCallbackListener(BCollisionObject.BICollisionCallbackEventHandler toBeRemoved)
        {
            if (m_collisionEventHandler != null) m_collisionEventHandler.DeregisterCollisionCallbackListener(toBeRemoved);
        }

        internal DiscreteDynamicsWorld m_ddWorld;
        internal CollisionWorld m_world;
        internal int m__frameCount = 0;
        internal float m_lastInterpolationTime = 0;
        internal float m_elapsedBetweenFixedFrames = 0;
        internal float m_fixedTimeStep;

        public void Awake()
        {
            m_fixedTimeStep = EventSystem.FixedUpdateTime;
            m_lastInterpolationTime = TimeHelper.ClientNowSeconds();
            m_elapsedBetweenFixedFrames = 0;
        }

        public virtual void FixedUpdate()
        {
            if (m_ddWorld != null)
            {
                /*  stepSimulation proceeds the simulation over 'timeStep', units in preferably in seconds.
                    By default, Bullet will subdivide the timestep in constant substeps of each 'fixedTimeStep'.
                    in order to keep the simulation real-time, the maximum number of substeps can be clamped to 'maxSubSteps'.
                    You can disable subdividing the timestep/substepping by passing maxSubSteps=0 as second argument to stepSimulation, but in that case you have to keep the timeStep constant. */

                float deltaTime = m_fixedTimeStep - m_elapsedBetweenFixedFrames;
                int numSteps = m_ddWorld.StepSimulation(deltaTime, 1, m_fixedTimeStep);
         
                m__frameCount += numSteps;
                m_lastInterpolationTime = TimeHelper.ClientNowSeconds();
                m_elapsedBetweenFixedFrames = 0f;
                numUpdates = 0;
            }

            //collisions
            if (m_collisionEventHandler != null)
            {
                m_collisionEventHandler.OnPhysicsStep(m_world);
            }
            //This is needed for rigidBody interpolation. The motion states will update the positions of the rigidbodies
            {
                float deltaTime = TimeHelper.ClientNowSeconds() - m_lastInterpolationTime;

                // We want to ensure that each bullet sim step corresponds to exactly one Unity FixedUpdate timestep
                if (deltaTime > 0f && (m_elapsedBetweenFixedFrames + deltaTime) < m_fixedTimeStep)
                {
                    m_elapsedBetweenFixedFrames += deltaTime;
                    int numSteps = m_ddWorld.StepSimulation(deltaTime, 1, m_fixedTimeStep);
                    m_lastInterpolationTime = TimeHelper.ClientNowSeconds();
                    numUpdates++;
                }
            }
        }

        int numUpdates = 0;

    }
}
