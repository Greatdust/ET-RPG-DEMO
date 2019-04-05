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
        public ICommandInput commandInput;
        public ICommandResult commandResult;
    }

    public static class CommandGCHelper
    {
        private static Dictionary<Type, Queue<ICommandInput>> commandInputs;
        private static Dictionary<Type, Queue<ICommandResult>> commandResults;

        private static Queue<Command> cacheCommands;

        static CommandGCHelper()
        {
            cacheCommands = new Queue<Command>();
            commandInputs = new Dictionary<Type, Queue<ICommandInput>>();
            commandResults = new Dictionary<Type, Queue<ICommandResult>>();
        }

        public static Command GetCommand()
        {
            if (cacheCommands.Count > 0)
            {
                return cacheCommands.Dequeue();
            }
            else
            {
                return new Command();
            }
        }

        public static T GetCommandInput<T>() where T : class, ICommandInput, new()
        {
            Type type = typeof(T);
            if (commandInputs.ContainsKey(type))
            {
                if (commandInputs[type].Count > 0)
                {
                    return commandInputs[type].Dequeue() as T;
                }
            }
            return new T();
        }

        public static T GetCommandResult<T>() where T : class, ICommandResult, new()
        {
            Type type = typeof(T);
            if (commandResults.ContainsKey(type))
            {
                if (commandResults[type].Count > 0)
                {
                    return commandResults[type].Dequeue() as T;
                }
            }
            return new T();
        }

        public static void Recycle(Command command)
        {
            if (cacheCommands.Count > 100)
            {
                return;
            }
            else
            {
                if (command.commandInput != null)
                    Recycle(command.commandInput);
                if(command.commandResult!=null)
                Recycle(command.commandResult);
                command.commandInput = null;
                command.commandResult = null;
                cacheCommands.Enqueue(command);
            }
        }

        private static void Recycle(ICommandInput commandInput)
        {
            Type type = commandInput.GetType();
            if (!commandInputs.ContainsKey(type))
            {
                commandInputs[type] = new Queue<ICommandInput>();
            }
            commandInputs[type].Enqueue(commandInput);
        }

        private static void Recycle(ICommandResult commandResult)
        {
            Type type = commandResult.GetType();
            if (!commandResults.ContainsKey(type))
            {
                commandResults[type] = new Queue<ICommandResult>();
            }
            commandResults[type].Enqueue(commandResult);
        }


    }
}
