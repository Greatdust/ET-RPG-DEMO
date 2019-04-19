using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public struct P_Rotation : IProperty<Quaternion>
    {
        public Quaternion Value { get; set; }

        public IProperty GetCopy()
        {
            return new P_Rotation()
            {
                Value = Value
            };
        }
    }

 


}
