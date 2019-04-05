using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{

    public class UnitStateDelta
    {
        public int frame;
        public Unit unit;
        public Dictionary<Type, ICommandResult> commandResults = new Dictionary<Type, ICommandResult>();
        
    }
}
