using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{

    public class CommandSimulaterComponent : Component
    {
        public Dictionary<Type, ICommandSimulater> commandSimulaters; //存储所有的Command模拟器
    }
}
