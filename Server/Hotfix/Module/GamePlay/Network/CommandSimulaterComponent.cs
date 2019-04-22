using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETHotfix
{
    [ObjectSystem]
    public class CommandSimulaterComponentAwakeSystem : AwakeSystem<CommandSimulaterComponent>
    {
        public override void Awake(CommandSimulaterComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class CommandSimulaterComponentLoadSystem : LoadSystem<CommandSimulaterComponent>
    {
        public override void Load(CommandSimulaterComponent self)
        {
            self.Load();
        }
    }


    public static class CommandSimulaterComponentSystem
    {

        public static void Awake(this CommandSimulaterComponent self)
        {
            self.Load();
        }

        public static void Load(this CommandSimulaterComponent self)
        {
            self.commandSimulaters = new Dictionary<Type, ICommandSimulater>();

            foreach (Type type in typeof(CommandSimulaterComponentSystem).Assembly.GetTypes())
            {

                object[] objects = type.GetCustomAttributes(typeof(CommandInputAttribute), false);
                if (objects.Length == 0)
                {
                    continue;
                }

                CommandInputAttribute commandInputAttr = (CommandInputAttribute)objects[0];

                ICommandSimulater simulater = Activator.CreateInstance(type) as ICommandSimulater;
                self.commandSimulaters.Add(commandInputAttr.Type, simulater);

            }
        }
    }
}
