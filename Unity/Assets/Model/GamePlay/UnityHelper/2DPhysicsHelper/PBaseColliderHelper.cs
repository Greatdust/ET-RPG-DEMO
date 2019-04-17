using Box2DSharp.Dynamics;
using Sirenix.OdinInspector;
using Sirenix.Serialization;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



public class PBaseColliderHelper : MonoBehaviour
{
    public BodyType bodyType = BodyType.DynamicBody;
    public bool isSensor;
    [HideInInspector]
    public ETModel.P2DBodyComponent bodyComponent;

    private void OnDrawGizmos()
    {
        OnDrawGizmosEvent();
    }

    protected virtual void OnDrawGizmosEvent() { }

}
