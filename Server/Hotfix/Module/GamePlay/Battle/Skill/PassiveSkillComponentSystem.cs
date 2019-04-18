using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using static PassiveSkillComponent;

namespace ETHotfix
{
    public static class PassiveSkillComponentSystem
    {
        static void ExcutePassiveSkill(this PassiveSkillComponent self ,string skillId)
        {

            Unit source = self.GetParent<Unit>();
            if (!self.bufferDatas.ContainsKey(skillId))
                self.bufferDatas[skillId] = new PassiveSkillBufferData();
            SkillHelper.ExcuteSkillParams excuteSkillParams = new SkillHelper.ExcuteSkillParams();
            excuteSkillParams.skillId = skillId;
            excuteSkillParams.source = self.GetParent<Unit>();
            excuteSkillParams.skillLevel = 1;

            self.cancelToken = new CancellationTokenSource();
            excuteSkillParams.cancelToken = self.cancelToken;

            PassiveSkillData passiveSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetPassiveSkill(skillId);
            if (passiveSkillData.listenToEvent)
            {
                if (!self.bufferDatas[skillId].apply)
                {
                    self.bufferDatas[skillId].apply = true;
                    self.bufferDatas[skillId].aEvent = new ListenPassiveSkillEvent(
                      (unitId) =>
                      {
                          if (unitId == source.Id)
                          {
                              if (SkillHelper.CheckIfSkillCanUse(skillId, source))
                              {
                                  self.tokenSource = new ETCancellationTokenSource();

                                  SkillHelper.ExcutePassiveSkill(excuteSkillParams);
                              }
                          }
                      }
                        );
                    Game.EventSystem.RegisterEvent(passiveSkillData.eventIdType, self.bufferDatas[skillId].aEvent);
                }
                return;
            }
            else
            {
                if (self.bufferDatas[skillId].apply) return;
            }
            if (SkillHelper.CheckIfSkillCanUse(skillId, source))
            {
                self.tokenSource = new ETCancellationTokenSource();
                SkillHelper.ExcutePassiveSkill(excuteSkillParams);
                self.bufferDatas[skillId].apply = true;
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

        public static void AddSkill(this PassiveSkillComponent self ,string skillId)
        {
            if (!self.skillList.ContainsKey(skillId))
            {
                self.skillList.Add(skillId, new BaseSkill_AppendedData() { level = 1 });
            }
            self.ExcutePassiveSkill(skillId);
        }

        public static void RemoveSkill(this PassiveSkillComponent self ,string skillId)
        {
            if (self.skillList.ContainsKey(skillId))
            {
                PassiveSkillData data = Game.Scene.GetComponent<SkillConfigComponent>().GetPassiveSkill(skillId);

                if (data.listenToEvent)
                {
                    if (self.bufferDatas.ContainsKey(data.skillId))
                    {
                        if (self.bufferDatas[data.skillId].apply)
                        {
                            self.bufferDatas[data.skillId].apply = false;
                            Game.EventSystem.RemoveEvent(data.eventIdType, self.bufferDatas[data.skillId].aEvent);
                        }
                    }

                }
                SkillHelper.OnPassiveSkillRemove(self.GetParent<Unit>(), skillId);
                self.skillList.Remove(skillId);
            }
        }

        public static BaseSkill_AppendedData GetSkill(this PassiveSkillComponent self, string skillId)
        {
            if (self.skillList.TryGetValue(skillId, out var data))
            {
                return data;
            }

            return null;
        }
    }
}
