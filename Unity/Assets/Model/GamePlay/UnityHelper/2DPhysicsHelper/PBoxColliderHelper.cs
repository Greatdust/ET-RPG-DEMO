using Box2DSharp.Common;
using ETModel;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class PBoxColliderHelper : PBaseColliderHelper
{
    public Vector3 size = Vector3.one;
    public Vector3 offset;

    private void Reset()
    {
        size = transform.localScale / 2;
        offset = Vector3.zero;
    }

    protected override void OnDrawGizmosEvent()
    {
        if (!Application.isPlaying)
        {
            PGizmosUtility.DebugDrawBox(transform.position + offset, Quaternion.Euler(new Vector3(0, transform.eulerAngles.y, 0)), Vector3.one, size, Color.red);
        }
        else
        {
            if (bodyComponent != null && !bodyComponent.IsDisposed)
            {
                System.Numerics.Vector2 pos = bodyComponent.body.GetPosition();
                float angle = bodyComponent.body.GetAngle();
                PGizmosUtility.DebugDrawBox(new Vector3(pos.X, bodyComponent.GetParent<Unit>().Position.y + bodyComponent.GetParent<Unit>().OffsetY, pos.Y), Quaternion.Euler(new Vector3(0, -angle * 180 / Settings.Pi, 0)), Vector3.one, size, Color.green);
            }
        }
    }

}
