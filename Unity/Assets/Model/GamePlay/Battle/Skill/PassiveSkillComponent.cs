using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

    public void Awake()
    {
        skillList = new Dictionary<string, BaseSkill_AppendedData>();
        bufferDatas = new Dictionary<string, PassiveSkillBufferData>();
    }

    void ExcutePassiveSkill(PassiveSkillData v)
    {

        Unit source = GetParent<Unit>();
        if (!bufferDatas.ContainsKey(v.skillId))
            bufferDatas[v.skillId] = new PassiveSkillBufferData();
        if (v.listenToEvent)
        {
            if (!bufferDatas[v.skillId].apply)
            {
                bufferDatas[v.skillId].apply = true;
                bufferDatas[v.skillId].aEvent = new ListenPassiveSkillEvent(
                  (unitId) =>
                    {
                        if (unitId == source.Id)
                        {
                            if (SkillHelper.CheckActiveConditions(v, source))
                            {
                                tokenSource = new ETCancellationTokenSource();
                                SkillHelper.ExcutePassiveSkill(v, tokenSource);
                                bufferDatas[v.skillId].apply = true;
                            }
                        }
                    }
                    );
                Game.EventSystem.RegisterEvent(v.eventIdType, bufferDatas[v.skillId].aEvent);
            }
            return;
        }
        else
        {
            if (bufferDatas[v.skillId].apply) return;
        }
        if (SkillHelper.CheckActiveConditions(v, source))
        {
            tokenSource = new ETCancellationTokenSource();
            SkillHelper.ExcutePassiveSkill(v, tokenSource);
            bufferDatas[v.skillId].apply = true;
        }

    }


    public class ListenPassiveSkillEvent : AEvent<long>
    {
        public Action<long> action;
        public ListenPassiveSkillEvent(Action<long> action)
        {
            this.action = action;
        }
        public override void Run(long a)
        {
            if (action != null)
                action(a);
        }
    }

    public void AddPassiveSkillData(string skillId)
    {
        if (!skillList.ContainsKey(skillId))
        {
            skillList.Add(skillId, new BaseSkill_AppendedData() { level = 1 });
        }
        PassiveSkillData data = Game.Scene.GetComponent<SkillConfigComponent>().GetPassiveSkill(skillId);
        ExcutePassiveSkill(data);
    }

    public void RemoveSkill(string skillId)
    {
        if (skillList.ContainsKey(skillId))
        {
            PassiveSkillData data = Game.Scene.GetComponent<SkillConfigComponent>().GetPassiveSkill(skillId);

            if (data.listenToEvent)
            {
                if (bufferDatas.ContainsKey(data.skillId))
                {
                    if (bufferDatas[data.skillId].apply)
                    {
                        bufferDatas[data.skillId].apply = false;
                        Game.EventSystem.RemoveEvent(data.eventIdType, bufferDatas[data.skillId].aEvent);
                    }
                }

            }
            skillList.Remove(skillId);
        }
    }

    public BaseSkill_AppendedData GetSkill(string skillId)
    {
        if (skillList.TryGetValue(skillId,out var data))
        {
            return data;
        }
        
        return null;
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

