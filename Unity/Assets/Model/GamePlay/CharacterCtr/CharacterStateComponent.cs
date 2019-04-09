using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        public enum CharacterGlobalStateType
        {
            InBattle,
            InDungeon,
            InScene
        }

        public CharacterGlobalStateType characterGlobalStateType;

        public bool CanDoAction { get; set; } = true;//是否可以行动,如果否,那么角色读条暂停
        public bool CanUseActiveSkill { get; set; } = true;//是否可以使用主动技能,如果否,那么角色只能使用普攻
        public bool DefensiveStance { get; set; }//是否是防御姿态
        public bool CanBeChoose { get; set; } = true;//是否可以被单体技能选中

        public void Awake()
        {

        }


    }
}
