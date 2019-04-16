using System;
using System.Collections.Generic;

namespace ETModel
{
    [ObjectSystem]
    public class CharacterStateComponentAwakeSystem : AwakeSystem<CharacterStateComponent>
    {
        public override void Awake(CharacterStateComponent self)
        {
            self.Awake();
        }
    }

    public class CharacterStateComponent : Component
    {
        public readonly Dictionary<int, bool> StateDic = new Dictionary<int, bool>();

        public void Awake()
        {
            // 这里初始化base值
            Set(SpecialStateType.UnStoppable, false);
            Set(SpecialStateType.NotInControl, false);
            Set(SpecialStateType.Invincible, false);
            Set(SpecialStateType.CantDoAction, false);
            Set(SpecialStateType.InBattle, true);
            Set(SpecialStateType.Die, false);
        }

        public void Set(SpecialStateType st, bool value)
        {
            StateDic[(int)st] = value;
            Game.EventSystem.Run(EventIdType.CharacterStateUpdate, st, value);
        }

        public bool Get(SpecialStateType st)
        {
            return StateDic[(int)st];
        }

      
    }
}