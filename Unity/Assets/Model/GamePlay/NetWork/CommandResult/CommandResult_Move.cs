using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{

    public class CommandResult_Move : ICommandResult
    {
        public Vector3 postion; //移动的目的地
        //目前游戏里的角色方向就是移动的方向,暂时不考虑方向
        //public Vector3 dir; //移动的方向 
    }
}
