using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    public struct P_UseSkill : IProperty<bool>
    {
        public bool Value { get ; set; }

        public IProperty GetCopy()
        {
            return new P_UseSkill()
            {
                Value = Value
            };
        }
    }

}
