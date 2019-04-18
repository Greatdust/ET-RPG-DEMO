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
    public class InputComponentUpdateSystem : UpdateSystem<InputComponent>
    {
        public override void Update(InputComponent self)
        {
            self.Update();
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

        public void Update()
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

        public Vector3 GetInputDir()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, Physics.AllLayers))
            {
                return new Vector3(hit.point.x - GetParent<Unit>().Position.x,0, hit.point.z - GetParent<Unit>().Position.z);
            }
            var forward = GetParent<Unit>().GameObject.transform.forward;
            return new Vector3(forward.x,0, forward.z);
        }

        public bool GetInputPos(out Vector3 pos)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100, Physics.AllLayers))
            {
                pos = hit.point;
                return true;
            }
            pos = Vector3.zero;
            return false;
        }

        public bool GetInputTarget(out Unit unit, bool findFriend, GroupIndex groupIndex)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, 100, LayerMask.GetMask("Character")))
            {
                UnitGameObjectHelper unitGameObject = hit.collider.gameObject.GetComponent<UnitGameObjectHelper>();
                Log.Debug("寻找目标");
                if (unitGameObject != null)
                {

                    Unit u = UnitComponent.Instance.Get(unitGameObject.UnitId);
                    if (findFriend)
                    {
                        if (u.UnitData.groupIndex == groupIndex)
                        {
                            unit = u;
                            return true;
                        }
                    }
                    else
                    {
                        if (u.UnitData.groupIndex != groupIndex)
                        {
                            unit = u;
                            return true;
                        }
                    }
                }
            }
            unit = null;
            return false;
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
            ActiveSkillComponent.Execute(skillId).Coroutine();
        }

    }
}