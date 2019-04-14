using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollsionHelper : UnityEngine.MonoBehaviour
{
    public long unitId;//附加本脚本的时候,会初始化的值
    public Action<long> onCollisionEnter;
    private void OnParticleCollision(GameObject other)
    {

        CollsionHelper collsionHelper = other.GetComponent<CollsionHelper>();

        onCollisionEnter(collsionHelper.unitId);


    }
}

