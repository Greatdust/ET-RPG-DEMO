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
        public Vector3 clickPos; //移动时点击的点,也是移动的目标位置
    }
}
