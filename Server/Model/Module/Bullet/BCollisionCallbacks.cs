using UnityEngine;
using System.Collections;
using BulletSharp;
using System;
using ETModel;

namespace BulletUnity
{
    public abstract class BCollisionCallbacks : Component, BCollisionObject.BICollisionCallbackEventHandler
    {
        public abstract void Start();

        public void OnEnable()
        {
            BCollisionObject co = GetParent<Entity>().GetComponent<BCollisionObject>();
            if (co != null)
            {
                co.AddOnCollisionCallbackEventHandler(this);
            }
        }

        public void OnDisable()
        {
            BCollisionObject co = GetParent<Entity>().GetComponent<BCollisionObject>();
            if (co != null)
            {
                co.RemoveOnCollisionCallbackEventHandler();
            }
        }

        public abstract void OnFinishedVisitingManifolds();

        public abstract void OnVisitPersistentManifold(PersistentManifold pm);
    }
}
