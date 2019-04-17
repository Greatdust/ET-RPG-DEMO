using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class CalNumericComponentAwakeSystem : AwakeSystem<CalNumericComponent>
    {
        public override void Awake(CalNumericComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class CalNumericComponentFixedUpdateSystem : FixedUpdateSystem<CalNumericComponent>
    {
        public override void FixedUpdate(CalNumericComponent self)
        {
            self.FixedUpdate();
        }
    }

    //客户端留一份,预备可能开发的单机模式 . 如果是有服务器的情况下,这个组件应该只有服务器用得到
    //这个组件用来结算 玩家 接收伤害/接收恢复效果 ,自身持续恢复效果
    //注意是结算!!!!!!!!!! 不是处理!!!!!!
    public class CalNumericComponent : Component
    {
        private const long restoreTimeSpan = 1000;//自动回复效果间隔,正式开发这个值请走配置文档

        private const float restorePct_NotInBattle = 0.05f;  //非战斗状态下每次回复百分比,正式开发这个值请走配置文档

        TimeSpanHelper.Timer timer;




        public void Awake()
        {
            timer = TimeSpanHelper.GetTimer(this.GetHashCode());
            timer.interval = restoreTimeSpan;
            timer.timing = TimeHelper.Now();
        }

        public void GetDamage(GameCalNumericTool.DamageData[] damageDatas)
        {
            int total = 0;
            foreach (var v in damageDatas)
            {
                total += v.damageValue;
            }
           // this.Entity.GetComponent<AnimatorComponent>().SetTrigger(CharacterAnim.Hit); //如果接下来还要被击飞之类的,导致播放对应的的动画. 这里不管.在Animator那里做好动画融合即可.

            Log.Debug("受到伤害  " + (-total));
            Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, GetParent<Unit>().Id, -(float)total); //受到伤害,所以是负数
        }

        public async void GetDie()
        {
            this.Entity.GetComponent<AnimatorComponent>().SetBoolValue(CharacterAnim.Dead, true);
            await TimerComponent.Instance.WaitAsync(2000); // 如果允许角色原地复活.这里就要加入取消了
           //TODO: 执行清理或者其他操作
        }

        public void GetHP(int value)
        {
            Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, GetParent<Unit>().Id, (float)value); //
        }

        public void GetMP(int value)
        {
            Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, GetParent<Unit>().Id, (float)value); //
        }

        //没法准确的在那一刻恢复,但是可以降低调用频率,尤其服务器,每个Update调用太耗了
        public void FixedUpdate()
        {
            if (TimeHelper.Now() - timer.timing >= timer.interval)
            {
                timer.timing += timer.interval;
                NumericComponent numericComponent = this.Entity.GetComponent<NumericComponent>();

                CharacterStateComponent unitStateComponent = this.Entity.GetComponent<CharacterStateComponent>();
                if (!unitStateComponent.Get(SpecialStateType.Die))
                    return;
                if (numericComponent.GetAsFloat(NumericType.HP_RemainPct) < 1.0f)
                {

                    if (unitStateComponent.Get( SpecialStateType.InBattle))
                    {
                        GetHP(numericComponent.GetAsInt(NumericType.HP_Restore));
                    }
                    else
                    {
                        GetHP(Mathf.RoundToInt(numericComponent.GetAsFloat(NumericType.HPMax_Final) * restorePct_NotInBattle));

                    }
                }
                if (numericComponent.GetAsFloat(NumericType.MP_RemainPct) < 1.0f)
                {

                    if (unitStateComponent.Get(SpecialStateType.InBattle))
                    {
                        GetMP(numericComponent.GetAsInt(NumericType.MP_Restore));
                    }
                    else
                    {
                        GetMP(Mathf.RoundToInt(numericComponent.GetAsFloat(NumericType.MPMax_Final) * restorePct_NotInBattle));
                    }
                }

            }

        }
    }


    #region Events

    [Event(EventIdType.GiveDamage)] 
    public class Unit_GiveDamageEvent : AEvent<long, GameCalNumericTool.DamageData[]>
    {
        public override void Run(long a, GameCalNumericTool.DamageData[] b)
        {
            var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();
            
            calNumeric.GetDamage(b);

        }
    }

    [Event(EventIdType.OnUnitDie)]
    public class Unit_GetDieEvent : AEvent<long>
    {
        public override void Run(long a)
        {
            var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();

            calNumeric.GetDie();

        }
    }

    [Event(EventIdType.GiveHealth)]
    public class Unit_GetHealthEvent : AEvent<long,int>
    {
        public override void Run(long a,int b)
        {
            var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();

            calNumeric.GetHP(b);

        }
    }

    [Event(EventIdType.GiveMp)]
    public class Unit_GetMPEvent : AEvent<long, int>
    {
        public override void Run(long a, int b)
        {
            var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();

            calNumeric.GetMP(b);

        }
    }


    #endregion
}