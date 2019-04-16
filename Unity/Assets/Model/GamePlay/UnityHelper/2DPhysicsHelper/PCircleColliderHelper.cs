using Box2DSharp.Common;
using ETModel;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class PCircleColliderHelper : PBaseColliderHelper
{
    public float radius = 1;
    public float height = 1;
    public Vector3 offset;

    protected override void OnDrawGizmosEvent()
    {
        if (!Application.isPlaying)
            PGizmosUtility.DebugDrawCylinder(transform.position + offset, Quaternion.identity, Vector3.one, radius, height, 1, Color.green);
        else
        {
            if (bodyComponent != null && !bodyComponent.IsDisposed)
            {
                System.Numerics.Vector2 pos = bodyComponent.body.GetPosition();
                float angle = bodyComponent.body.GetAngle();
                PGizmosUtility.DebugDrawCylinder(new Vector3(pos.X, bodyComponent.GetParent<Unit>().Position.y + bodyComponent.GetParent<Unit>().OffsetY, pos.Y), Quaternion.Euler(new Vector3(0, -angle * 180 / Settings.Pi, 0)), Vector3.one, radius, height, 1, Color.green);
            }
        }
    }

}
