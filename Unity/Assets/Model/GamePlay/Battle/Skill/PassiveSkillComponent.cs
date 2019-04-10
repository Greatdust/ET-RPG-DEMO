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
    public Dictionary<string, PassiveSkillData> passiveSkillDic;

    private List<Action> applyAction = new List<Action>();

    private float timeSpan = 0.2f;


    public void Awake()
    {
        passiveSkillDic = new Dictionary<string, PassiveSkillData>();
    }

    //public void Update()
    //{
    //    TimeSpanHelper.Timer timer= TimeSpanHelper.GetTimer(this.GetHashCode());
    //    if (timer.remainTime > 0)
    //    {
    //        timer.remainTime -= Time.deltaTime;
    //        return;
    //    }
    //    if (BattleMgrComponent.Instance == null) return;
    //    if (!BattleMgrComponent.Instance.battleStart) return;
    //    try
    //    {
    //        ExcutePassiveSkill();
    //    }
    //    catch (Exception e)
    //    {
    //        Log.Error(e.ToString());
    //    }
    //    if (timer.remainTime <= 0)
    //    {
    //        timer.remainTime = timeSpan;
    //        return;
    //    }
    //}

    async void ExcutePassiveSkill(PassiveSkillData v)
    {

        Unit source = GetParent<Unit>();

        if (v.listenToEvent)
        {
            if (!v.apply)
            {
                v.apply = true;
                v.aEvent = new ListenPassiveSkillEvent(
                    async (unitId) =>
                    {
                        if (unitId == source.Id)
                        {
                            if (SkillHelper.CheckActiveConditions(v, source))
                            {

                                SkillHelper.ExcutePassiveSkill(v);
                                v.apply = true;
                            }
                        }
                    }
                    );
                Game.EventSystem.RegisterEvent(v.eventIdType, v.aEvent);
            }
            return;
        }
        else
        {
            if (v.apply) return;
        }
        if (SkillHelper.CheckActiveConditions(v, source))
        {
            SkillHelper.ExcutePassiveSkill(v);
            v.apply = true;
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

    public void AddPassiveSkillData(PassiveSkillData passiveSkillData)
    {
        passiveSkillData.SourceUnit = GetParent<Unit>();
        passiveSkillDic[passiveSkillData.skillId] = passiveSkillData;
        ExcutePassiveSkill(passiveSkillData);
        passiveSkillData.buffReturnValues = new Dictionary<string, List<IBuffReturnedValue>>();
    }

    public void RemoveSkill(string skillId)
    {
        PassiveSkillData passiveSkillData = null;
        if (passiveSkillDic.TryGetValue(skillId, out passiveSkillData))
        {
            if (passiveSkillData.listenToEvent)
            {
                if (passiveSkillData.apply)
                {
                    passiveSkillData.apply = false;
                    Game.EventSystem.RemoveEvent(passiveSkillData.eventIdType, passiveSkillData.aEvent);
                }

            }
            passiveSkillDic.Remove(skillId);
        }
    }

    public PassiveSkillData GetSkill(string skillId)
    {
        PassiveSkillData data;
        if (!passiveSkillDic.TryGetValue(skillId, out data))
        {
            return null;
        }
        return data;
    }

}

