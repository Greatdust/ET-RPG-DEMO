using PF;
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
        public List<Vector3> Path; //移动的路径点列表. 服务器发下路径的时候,不会一次性发全
        //目前游戏里的角色方向就是移动的方向,暂时不考虑方向
        //public Vector3 dir; //移动的方向 
    }
}
