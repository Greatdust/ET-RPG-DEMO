using System;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace ETModel
{

    [ObjectSystem]
    public class InputComponentAwakeSystem : AwakeSystem<InputComponent>
    {
        public override void Awake(InputComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class InputComponentUpdateSystem : FixedUpdateSystem<InputComponent>
    {
        public override void FixedUpdate(InputComponent self)
        {
            self.FixedUpdate();
        }
    }

    public class InputComponent : Component
    {      
        public class HotKeyState
        {
            public string skillId;
            public bool canUse;
        }

        public Dictionary<string, HotKeyState> hotKeyToSkill;//快捷键对技能的映射

        private ActiveSkillComponent ActiveSkillComponent;
        private Unit parent;

        public void Awake()
        {
            hotKeyToSkill = new Dictionary<string, HotKeyState>();
            parent = GetParent<Unit>();
            ActiveSkillComponent = parent.GetComponent<ActiveSkillComponent>();
        }

        public void FixedUpdate()
        {
            if (Input.anyKeyDown)
            {
                foreach (var v in hotKeyToSkill)
                {
                    if (Input.GetButtonDown(v.Key))
                    {
                        v.Value.canUse = SkillHelper.CheckIfSkillCanUse(v.Value.skillId, parent);
                        if (v.Value.canUse)
                            UseSkill(v.Value.skillId);
                    }
                }
            }
        }

        public void AddSkillToHotKey(string hotKeyName, string skillId)
        {
            hotKeyToSkill[hotKeyName] = new HotKeyState()
            {
                skillId = skillId
            };
        }

        public void RemoveSkill(string hotKeyName)
        {
            if (hotKeyToSkill.ContainsKey(hotKeyName))
            {
                hotKeyToSkill.Remove(hotKeyName);
            }
        }

        public void UseSkillByHotKey(string hotKeyName)
        {
            HotKeyState hotKeyState = null;
            if (!hotKeyToSkill.TryGetValue(hotKeyName, out hotKeyState))
            {
                return;
            }
            if (!hotKeyState.canUse)
            {
                return;
            }
            UseSkill(hotKeyState.skillId);
        }

        private void UseSkill(string skillId)
        {
            ActiveSkillComponent.Excute(skillId).Coroutine();
        }

    }
}