using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public struct P_Position : IProperty<Vector3>
    {
        public Vector3 Value { get ; set; }

        public IProperty GetCopy()
        {
            return new P_Position()
            {
                Value = Value
            };
        }
    }

}
