using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
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


    public class CommandSimulaterComponent : Component
    {
        public Dictionary<Type, ICommandSimulater> commandSimulaters; //存储所有的Command模拟器

        public void Awake()
        {
            Load();
        }

        public void Load()
        {
            this.commandSimulaters = new Dictionary<Type, ICommandSimulater>();

            foreach (Type type in this.GetType().Assembly.GetTypes())
            {

                object[] objects = type.GetCustomAttributes(typeof(CommandInputAttribute), false);
                if (objects.Length == 0)
                {
                    continue;
                }

                CommandInputAttribute commandInputAttr = (CommandInputAttribute)objects[0];

                ICommandSimulater simulater = Activator.CreateInstance(type) as ICommandSimulater;
                commandSimulaters.Add(commandInputAttr.Type, simulater);

            }
        }
    }
}
