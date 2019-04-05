using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    public static  class CommandResultPool 
    {
        private static readonly Dictionary<Type, Queue<ICommandResult>> resultPool = new Dictionary<Type, Queue<ICommandResult>>();

        //public ICommandResult Get(Type type)
        //{
        //    if(resultPool)
        //}
    }
}
