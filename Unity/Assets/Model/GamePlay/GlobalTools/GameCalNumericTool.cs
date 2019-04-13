using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


[Event(EventIdType.CalDamage)]
public class CalFinalDamageEvent : AEvent<long, long, GameCalNumericTool.DamageData>
{
    public override void Run(long a, long b, GameCalNumericTool.DamageData c)
    {
        GameCalNumericTool.CalFinalDamage(a, b, c);
    }
}

public static class GameCalNumericTool
{
    public enum DamageType
    {
        physic,
        magic
    }
    public struct DamageData
    {
        public DamageType damageType;
        public int damageValue;//伤害值
        public bool isCritical;//是否是暴击伤害
    }
    /// <summary>
    /// 返回实际上的下一次行动的行动周期时间
    /// </summary>
    /// <param name="baseInterval"></param>
    /// <param name="unitSpeed"></param>
    /// <param name="timeImpactByUnitAction"></param>
    /// <returns></returns>
    public static float GetActionInterval(float baseInterval,int unitSpeed,float timeImpactByUnitAction)
    {
        float minInterval = 0.3f;
        float unitTime = (baseInterval + timeImpactByUnitAction) * (1 - (unitSpeed / (100 + unitSpeed)));
        if (unitTime <= minInterval)
        {
            unitTime = minInterval;
        }
        return unitTime;
    }

    public static void CalFinalDamage(long sourceUnitId,long destUnitId,DamageData skillDamageValue)
    {
        try
        {
            UnitStateComponent unitState = UnitComponent.Instance.Get(destUnitId).GetComponent<UnitStateComponent>();
            Property_Die property_Die = unitState.GetCurrState<Property_Die>();
            if (property_Die.Get()) return;
            Property_Invicible property_Invicible = unitState.GetCurrState<Property_Invicible>();
            if (property_Invicible.Get())
            {
                //TODO: 提示无敌状态
                return;
            }

            NumericComponent sourceUnitNumericCom = UnitComponent.Instance.Get(sourceUnitId).GetComponent<NumericComponent>();
            NumericComponent destUnitNumericCom = UnitComponent.Instance.Get(destUnitId).GetComponent<NumericComponent>();
            int rateCharge = 0;
            //命中判定
            float hitRate = sourceUnitNumericCom.GetAsFloat(NumericType.HitRate) - destUnitNumericCom.GetAsFloat(NumericType.DodgeRate);
            rateCharge = RandomHelper.RandomNumber(0, 100);
            if (rateCharge / 100.0f > hitRate)
            {
                Game.EventSystem.Run(EventIdType.AttackMissing, sourceUnitId, destUnitId);
                return;
            }

            //暴击判定
            //可能有技能提升效果
            if(!skillDamageValue.isCritical)
            {
                float criticalRate = sourceUnitNumericCom.GetAsFloat(NumericType.CritRate);
                rateCharge = RandomHelper.RandomNumber(0, 100);
                if (rateCharge / 100.0f <= criticalRate)
                {
                    //暴击判定通过
                    skillDamageValue.isCritical = true;
                 
                }
            }
            if (skillDamageValue.isCritical)
            {
                skillDamageValue.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * sourceUnitNumericCom.GetAsFloat(NumericType.CritDamagePct));
            }

         

            NumericType resistType  = NumericType.ArmorResist;

            if (skillDamageValue.damageType != DamageType.physic)
            {
                resistType = NumericType.MagicResist;

            }

            skillDamageValue.damageValue = Mathf.RoundToInt((skillDamageValue.damageValue )* (1 - 100 / (destUnitNumericCom.GetAsFloat(resistType) + 100)));


            //预防可能要考虑什么白字红字,黑字粉字等乱七八糟的情况,所以专门用一个List
            DamageData[] array = new DamageData[1];
            array[0] = skillDamageValue;

            //计算最终伤害加成,减免

            for (int i = 0; i < array.Length; i++)
            {
                var damage = array[i];
                damage.damageValue = Mathf.RoundToInt(array[i].damageValue *
                    (1 + sourceUnitNumericCom.GetAsFloat(NumericType.FinalDamage_AddPct) - destUnitNumericCom.GetAsFloat(NumericType.FinalDamage_ReducePct)));
                //限定最小伤害0
                damage.damageValue = Mathf.Clamp(array[i].damageValue, 0, int.MaxValue);
                array[i] = damage;
            }


            //给予伤害
            Game.EventSystem.Run(EventIdType.GiveDamage, destUnitId, array);

            //给予吸血,吸法
            float xiQu = sourceUnitNumericCom.GetAsFloat(NumericType.HP_LeechRate);
            if (xiQu > 0)
                Game.EventSystem.Run(EventIdType.GiveHealth, sourceUnitId, Mathf.RoundToInt(skillDamageValue.damageValue * xiQu ));
            xiQu = sourceUnitNumericCom.GetAsFloat(NumericType.MP_LeechRate);
            if (xiQu > 0)
                Game.EventSystem.Run(EventIdType.GiveMp, sourceUnitId, Mathf.RoundToInt(skillDamageValue.damageValue * xiQu));
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public static void CalDotDamage(long sourceUnitId,  Unit destUnit, Buff_DOT dot)
    {
        DamageData damageData = new DamageData();
        damageData.damageType = dot.damageType;

        damageData.damageValue =  BuffHandlerVar.cacheDatas_int[(sourceUnitId,dot.buffSignal)];
        NumericComponent destUnitNumericCom = destUnit.GetComponent<NumericComponent>();
        NumericType resistType = NumericType.ArmorResist;

        if (dot.damageType != DamageType.physic)
        {
            resistType = NumericType.MagicResist;

        }

        damageData.damageValue = Mathf.RoundToInt(damageData.damageValue * (1 - 100 / (destUnitNumericCom.GetAsFloat(resistType) + 100)));
        damageData.damageValue = Mathf.RoundToInt(damageData.damageValue *
               (1 - destUnitNumericCom.GetAsFloat(NumericType.FinalDamage_ReducePct)));

        DamageData[] array = new DamageData[1];
        array[0] = damageData;
        Game.EventSystem.Run(EventIdType.GiveDamage, destUnit.Id, array);
    }

    public static void CalRestore(Unit sourceUnit)
    {
        NumericComponent unitNumericCom = sourceUnit.GetComponent<NumericComponent>();
        int hp = unitNumericCom.GetAsInt(NumericType.HP_Restore);
        if (hp <= 0)
        {
            //流血等状态不通过这里计算,而是单独用DOT计算. 因为流血和自然恢复可以共存
            return;
        }
        Game.EventSystem.Run(EventIdType.GiveHealth, sourceUnit.Id, Mathf.RoundToInt(hp));
        int mp = unitNumericCom.GetAsInt(NumericType.MP_Restore);
        if (mp <= 0)
        {
            return;
        }
        Game.EventSystem.Run(EventIdType.GiveMp, sourceUnit.Id, mp);
    }


}

