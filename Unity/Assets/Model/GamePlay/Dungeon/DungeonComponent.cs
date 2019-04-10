using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;

namespace ETModel
{
    [ObjectSystem]
    public class DungeonComponentAwakeSystem : AwakeSystem<DungeonComponent,int>
    {
        public override void Awake(DungeonComponent self,int dungeonId)
        {
            self.Awake(dungeonId);
        }
    }


    public class DungeonComponent : Component
    {

        public List<LevelConfigData> currDungeonData = new List<LevelConfigData>();

        public int currLevelIndex;//正在进行哪个关卡

        public long dungeonTiming;
        public long levelTiming;


        public List<Unit> playerTeam;
        public List<Unit> enemyTeam;

        public void Awake(int dungeonId)
        {

        }

        public void Load()
        {

        }

        public void Deserializer(string path)
        {
            using (FileStream fileStream = File.OpenRead(path))
            {
                //二进制格式化程序
                BinaryFormatter bf = new BinaryFormatter();
                //反序列化
                currDungeonData = (List<LevelConfigData>)bf.Deserialize(fileStream);

                fileStream.Close();
            }
        }
    }
}
