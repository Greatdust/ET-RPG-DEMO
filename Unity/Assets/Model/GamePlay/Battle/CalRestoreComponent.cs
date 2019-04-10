using System.Collections;
using System.Collections.Generic;
using ETModel;
using UnityEngine;
[ObjectSystem]
public class CalRestoreComponentAwakeSystem : AwakeSystem<CalRestoreComponent>
{
    public override void Awake(CalRestoreComponent self)
    {
        self.Awake();
    }
}

[ObjectSystem]
public class CalRestoreComponentFixedUpdateSystem : FixedUpdateSystem<CalRestoreComponent>
{
    public override void FixedUpdate(CalRestoreComponent self)
    {
       self.FixedUpdate();
    }
}

//客户端留一份,预备可能开发的单机模式 . 如果是有服务器的情况下,这个组件应该只有服务器用得到
public class CalRestoreComponent : ETModel.Component
{
    private const long restoreTimeSpan = 1000;//自动回复效果间隔,正式开发这个值请走配置文档

    private const float restorePct_NotInBattle = 0.05f;  //非战斗状态下每次回复百分比,正式开发这个值请走配置文档

    TimeSpanHelper.Timer timer;


    #region public


    public void Awake()
    {
        timer = TimeSpanHelper.GetTimer(this.GetHashCode());
        timer.interval = restoreTimeSpan;
        timer.timing = TimeHelper.Now();
    }

    //没法准确的在那一刻恢复,但是可以降低调用频率,尤其服务器,每个Update调用太耗了
    public void FixedUpdate()
    {
        if (TimeHelper.Now() - timer.timing >= timer.interval)
        {
            timer.timing += timer.interval;
            NumericComponent numericComponent = this.Entity.GetComponent<NumericComponent>();

            UnitStateComponent unitStateComponent = this.Entity.GetComponent<UnitStateComponent>();

            Property_InBattleState property_InBattleState = unitStateComponent.GetCurrState<Property_InBattleState>();
            if (numericComponent.GetAsFloat(NumericType.HP_RemainPct) < 1.0f)
            {

                if (property_InBattleState.Get())
                {
                    Game.EventSystem.Run(EventIdType.GiveHealth, GetParent<Unit>().Id, numericComponent.GetAsInt(NumericType.HP_Restore));
                }
                else
                {
                    Game.EventSystem.Run(EventIdType.GiveHealth, GetParent<Unit>().Id, Mathf.RoundToInt(numericComponent.GetAsFloat(NumericType.HPMax_Final) * restorePct_NotInBattle));
                }
            }
            if (numericComponent.GetAsFloat(NumericType.MP_RemainPct) < 1.0f)
            {

                if (property_InBattleState.Get())
                {
                    Game.EventSystem.Run(EventIdType.GiveMp, GetParent<Unit>().Id, numericComponent.GetAsInt(NumericType.MP_Restore));
                }
                else
                {
                    Game.EventSystem.Run(EventIdType.GiveMp, GetParent<Unit>().Id, Mathf.RoundToInt(numericComponent.GetAsFloat(NumericType.MPMax_Final) * restorePct_NotInBattle));
                }
            }

        }

    }

    #endregion








}
