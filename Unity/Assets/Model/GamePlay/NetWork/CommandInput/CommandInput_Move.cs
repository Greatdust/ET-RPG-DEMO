using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{

    public class CommandInput_Move : ICommandInput
    {
        public Vector3 moveDir; //移动输入的方向
    }
}
