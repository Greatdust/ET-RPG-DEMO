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

    void ExcutePassiveSkill(string skillId)
    {

        Unit source = GetParent<Unit>();
        if (!bufferDatas.ContainsKey(skillId))
            bufferDatas[skillId] = new PassiveSkillBufferData();
        SkillHelper.ExecuteSkillParams excuteSkillParams = new SkillHelper.ExecuteSkillParams();
        excuteSkillParams.skillId = skillId;
        excuteSkillParams.source = GetParent<Unit>();
        excuteSkillParams.skillLevel = 1;

        cancelToken = new CancellationTokenSource();
        excuteSkillParams.cancelToken = cancelToken;

        PassiveSkillData passiveSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetPassiveSkill(skillId);
        if (passiveSkillData.listenToEvent)
        {
            if (!bufferDatas[skillId].apply)
            {
                bufferDatas[skillId].apply = true;
                bufferDatas[skillId].aEvent = new ListenPassiveSkillEvent(
                  (unitId) =>
                    {
                        if (unitId == source.Id)
                        {
                            if (SkillHelper.CheckIfSkillCanUse(skillId, source))
                            {
                                tokenSource = new ETCancellationTokenSource();

                                SkillHelper.ExecutePassiveSkill(excuteSkillParams);
                            }
                        }
                    }
                    );
                Game.EventSystem.RegisterEvent(passiveSkillData.eventIdType, bufferDatas[skillId].aEvent);
            }
            return;
        }
        else
        {
            if (bufferDatas[skillId].apply) return;
        }
        if (SkillHelper.CheckIfSkillCanUse(skillId, source))
        {
            tokenSource = new ETCancellationTokenSource();
            SkillHelper.ExecutePassiveSkill(excuteSkillParams);
            bufferDatas[skillId].apply = true;
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

    public void AddSkill(string skillId)
    {
        if (!skillList.ContainsKey(skillId))
        {
            skillList.Add(skillId, new BaseSkill_AppendedData() { level = 1 });
        }
        ExcutePassiveSkill(skillId);
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
            SkillHelper.OnPassiveSkillRemove(GetParent<Unit>(),skillId);
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

