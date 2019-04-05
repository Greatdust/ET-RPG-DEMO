using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    

    public interface ICommandInput
    { }

    public interface ICommandResult
    { }

    public interface ICommandSimulater
    {
        ICommandResult Simulate(ICommandInput commandInput, Unit unit);
    }

    public class Command
    {
        public uint sequence;//记录指令序号
        public ICommandInput commandInput;
        public ICommandResult commandResult;
    }
}
