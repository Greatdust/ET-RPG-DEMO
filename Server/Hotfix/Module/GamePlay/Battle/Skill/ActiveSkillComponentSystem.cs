using ETModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ETHotfix
{
    public static class ActiveSkillComponentSystem
    {


        public static async ETVoid Excute(this ActiveSkillComponent self, string skillId)
        {
            try
            {
                if (!self.skillList.ContainsKey(skillId)) return;
                if (!SkillHelper.CheckIfSkillCanUse(skillId, self.GetParent<Unit>())) return;
                ActiveSkillData activeSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(skillId);
                SkillHelper.ExcuteSkillParams excuteSkillParams = new SkillHelper.ExcuteSkillParams();
                excuteSkillParams.skillId = skillId;
                excuteSkillParams.source = self.GetParent<Unit>();
                excuteSkillParams.skillLevel = 1;
                bool canUse = await SkillHelper.CheckInput(excuteSkillParams);
                if (!canUse) return;

                //TODO: 暂时先让使用技能的取消之前的行动. 后续需要根据情况判断是否处于前一个技能的硬直/引导等状态
                self.cancelToken?.Cancel();
                Game.EventSystem.Run(EventIdType.CancelPreAction, self.GetParent<Unit>());
                CharacterStateComponent characterStateComponent = self.GetParent<Unit>().GetComponent<CharacterStateComponent>();
                characterStateComponent.Set(SpecialStateType.NotInControl, true);
                self.cancelToken = new CancellationTokenSource();
                excuteSkillParams.cancelToken = self.cancelToken;
                await SkillHelper.ExcuteActiveSkill(excuteSkillParams);
                self.cancelToken?.Dispose();
                self.cancelToken = null;
                characterStateComponent.Set(SpecialStateType.NotInControl, false);
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
            }

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
}
