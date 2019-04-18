using PF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{

    public class CommandResult_UseSkill : ICommandResult
    {
        public string skillId; //对应的技能Id
        public bool success;//允许使用
    }
}
