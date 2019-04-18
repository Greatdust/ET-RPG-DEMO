using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


[ObjectSystem]
public class PassiveSkillComponentAwakeSystem : AwakeSystem<PassiveSkillComponent>
{
    public override void Awake(PassiveSkillComponent self)
    {
        self.Awake();
    }
}

//[ObjectSystem]
//public class PassiveSkillComponentUpdateSystem : UpdateSystem<PassiveSkillComponent>
//{
//    public override void Update(PassiveSkillComponent self)
//    {
//       // self.Update();
//    }
//}

/// <summary>
/// 每个战斗单位身上都会有的一个被动技能组件，用以管理单位身上的被动技能
/// </summary>
public class PassiveSkillComponent : ETModel.Component
{
    public Dictionary<string, BaseSkill_AppendedData> skillList;
    public ETCancellationTokenSource tokenSource;

    public class PassiveSkillBufferData
    {
        public bool apply;
        public AEvent<long> aEvent;
    }

    public Dictionary<string, PassiveSkillBufferData> bufferDatas;

    public CancellationTokenSource cancelToken;//用以执行技能中断的

    public void Awake()
    {
        skillList = new Dictionary<string, BaseSkill_AppendedData>();
        bufferDatas = new Dictionary<string, PassiveSkillBufferData>();
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        base.Dispose();
        bufferDatas.Clear();
        bufferDatas = null;
    }

}

