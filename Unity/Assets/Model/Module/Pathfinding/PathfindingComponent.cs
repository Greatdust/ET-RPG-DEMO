using PF;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class PathfindingComponentAwakeSystem : AwakeSystem<PathfindingComponent>
    {
        public override void Awake(PathfindingComponent self)
        {
            self.PathReturnQueue = new PathReturnQueue(self);
            self.PathProcessor = new PathProcessor(self.PathReturnQueue, 1, false);
            
            // 读取寻路配置
            self.AStarConfig = new AStarConfig(); //MongoHelper.FromJson<AStarConfig>(File.ReadAllText("./pathfinding.config"));
            self.AStarConfig.pathProcessor = self.PathProcessor;

            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("pathfind.unity3d");
            TextAsset textAsset = Game.Scene.GetComponent<ResourcesComponent>().GetAsset("pathfind.unity3d", "graph") as TextAsset;
            //测试
            self.AStarConfig.graphs = DeserializeHelper.Load(textAsset.bytes);
        }
    }
    
    public class PathfindingComponent: Component
    {
        public PathReturnQueue PathReturnQueue;
        
        public PathProcessor PathProcessor;

        public AStarConfig AStarConfig;
        
        public bool Search(ABPathWrap path)
        {
            this.PathProcessor.queue.Push(path.Path);
            while (this.PathProcessor.CalculatePaths().MoveNext())
            {
                if (path.Path.CompleteState != PathCompleteState.NotCalculated)
                {
                    break;
                }
            }

            if (path.Path.CompleteState != PathCompleteState.Complete)
            {
                return false;
            }
            
            PathModifyHelper.StartEndModify(path.Path);
            PathModifyHelper.FunnelModify(path.Path);

            return true;
        }
    }
}