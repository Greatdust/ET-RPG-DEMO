using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class UnitStateMgrComponentAwakeSystem : AwakeSystem<UnitStateMgrComponent>
    {
        public override void Awake(UnitStateMgrComponent self)
        {
            self.Awake();
        }
    }


    public class UnitStateMgrComponent : Component
    {

        public List<UnitStateComponent> unitStateComponents; //管理所有角色的UnitStateComponent

        public int currFrame;//服务器当前帧,用以确定现在这一刻的状态

        //这两个用来做发射间隔用
        public const int sendMsgDelta = 3;//间隔3帧发一次数据

        public void Awake()
        {
            currFrame = 0;
            unitStateComponents = new List<UnitStateComponent>();

        }

        public void Add(UnitStateComponent unitStateComponent)
        {
            unitStateComponents.Add(unitStateComponent);
        }

        public void Remove(UnitStateComponent unitStateComponent)
        {
            unitStateComponents.Remove(unitStateComponent);
        }

        public override void Dispose()
        {
            if (IsDisposed)
                return;
            base.Dispose();
            unitStateComponents.Clear();
        }

    }
}
