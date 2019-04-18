using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{

    public class CommandInput_UseSkill : ICommandInput
    {
        public string skillId;
        public string pipelineSignal;//输入环节的pipelineSignal
        public IBufferValue bufferValue;//输入的内容
    }
}
