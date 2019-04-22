using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;


    public static class ActiveSkillComponentSystem
    {

        public static bool CheckCanUse(this ActiveSkillComponent self, string skillId)
        {
            if (self.usingSkill
          || !self.skillList.ContainsKey(skillId)
          || !SkillHelper.CheckIfSkillCanUse(skillId, self.GetParent<Unit>()))
            {

                return false;
            }
            return true;
        }

        public static async ETVoid Execute(this ActiveSkillComponent self, string skillId)
        {
            ActiveSkillData activeSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(skillId);
            SkillHelper.ExecuteSkillParams excuteSkillParams = new SkillHelper.ExecuteSkillParams();
            excuteSkillParams.skillId = skillId;
            excuteSkillParams.source = self.GetParent<Unit>();
            excuteSkillParams.skillLevel = 1;

            self.currUsingSkillId = skillId;

            //TODO: 暂时先直接取消之前的行动
 
            self.cancelToken?.Cancel();
            Game.EventSystem.Run(EventIdType.CancelPreAction, self.GetParent<Unit>());
            CharacterStateComponent characterStateComponent = self.GetParent<Unit>().GetComponent<CharacterStateComponent>();
            characterStateComponent.Set(SpecialStateType.NotInControl, true);
            self.cancelToken = new CancellationTokenSource();
            excuteSkillParams.cancelToken = self.cancelToken;
            
            await SkillHelper.ExecuteActiveSkill(excuteSkillParams);
            self.cancelToken = null;
            characterStateComponent.Set(SpecialStateType.NotInControl, false);
            self.currUsingSkillId = string.Empty;


        }


    //中断可能正在执行的技能
    public static void Interrupt(this ActiveSkillComponent self, TypeOfInterruption type)
    {
        //TODO: 根据当前使用技能允许的可打断类型判定打断是否可以成功

        CharacterStateComponent characterStateComponent = self.GetParent<Unit>().GetComponent<CharacterStateComponent>();
        if (characterStateComponent.Get(SpecialStateType.UnStoppable)) return;// 霸体状态,打断失败
        self.cancelToken?.Cancel();
        self.cancelToken = null;
        M2C_InterruptSkill m2c = new M2C_InterruptSkill();
        m2c.Frame = Game.Scene.GetComponent<UnitStateMgrComponent>().currFrame;
        m2c.Id = self.GetParent<Unit>().Id;
        ETHotfix.MessageHelper.Broadcast(m2c);

    }


        public static void AddSkill(this ActiveSkillComponent self, string skillId)
        {
            ActiveSkillData activeSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(skillId);
            if (activeSkillData.isNormalAttack)
            {
                self.Skill_NormalAttack = skillId;
            }
            if (!self.skillList.ContainsKey(skillId))
            {
                self.skillList.Add(skillId, new BaseSkill_AppendedData() { level = 1 });
            }
        }

        public static void RemoveSkill(this ActiveSkillComponent self, string skillId)
        {
            if (!self.skillList.ContainsKey(skillId)) return;
            if (skillId == self.Skill_NormalAttack) return;
            self.skillList.Remove(skillId);
        }

        public static BaseSkill_AppendedData GetSkillAppendedData(this ActiveSkillComponent self, string skillId)
        {
            if (self.skillList.TryGetValue(skillId, out var data))
                return data;
            else
                return null;
        }
    }

