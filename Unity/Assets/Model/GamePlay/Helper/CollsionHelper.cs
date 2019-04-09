using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class CollsionHelper : UnityEngine.MonoBehaviour
{
    public long unitId;//附加本脚本的时候,会初始化的值
    public List<Action<long>> onCollisionEnter =new List<Action<long>>();
    private void OnParticleCollision(GameObject other)
    {
        if (onCollisionEnter.Count > 0)
        {
            CollsionHelper collsionHelper = other.GetComponent<CollsionHelper>();
            foreach (var v in onCollisionEnter)
            {
                v(collsionHelper.unitId);
            }
        }
    }
}

