using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public class CommandInputAttribute : BaseAttribute
    {
        public Type Type;
        public CommandInputAttribute(Type type)
        {
            this.Type = type;
        }
    }
}
