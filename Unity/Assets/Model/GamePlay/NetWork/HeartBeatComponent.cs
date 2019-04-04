using ETModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class HeartBeatComponentUpdateComponent : UpdateSystem<HeartBeatComponent>
    {
        public override void Update(HeartBeatComponent self)
        {
            self.Update();
        }
    }


    public class HeartBeatComponent : Component
    {
        public float sendInterval = 2;
        public float timing = 0;


        public async void Update()
        {
            timing += Time.deltaTime;
            if (timing > sendInterval)
            {
                timing = 0;
                // 开始发包
                try
                {
                    R2C_HeartBeat  result = (R2C_HeartBeat)await ETModel.Game.Scene.GetComponent<SessionComponent>().Session.Call(new C2R_HeartBeat());
                }
                catch
                {
                    // 执行断线后的操作
                   

                }
            }

        }
    }
}
