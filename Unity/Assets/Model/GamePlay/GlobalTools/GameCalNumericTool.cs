using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class GameCalNumericTool
{
    public enum DamageType
    {
        Physic,
        Magic,
        //TODO 以后再加个真实伤害
    }
    public struct DamageData
    {
        public DamageType damageType;
        public int damageValue;//伤害值
        public bool isCritical;//是否是暴击伤害
    }

    public static bool CalFinalDamage(long sourceUnitId,long destUnitId,DamageData skillDamageValue)
    {
        try
        {
            
            CharacterStateComponent unitState = UnitComponent.Instance.Get(destUnitId).GetComponent<CharacterStateComponent>();
            if (unitState.Get( SpecialStateType.Die)) return false;

            if (unitState.Get( SpecialStateType.Invincible))
            {
                //TODO: 提示无敌状态
                return false;
            }
            NumericComponent sourceUnitNumericCom = UnitComponent.Instance.Get(sourceUnitId).GetComponent<NumericComponent>();
            NumericComponent destUnitNumericCom = UnitComponent.Instance.Get(destUnitId).GetComponent<NumericComponent>();
            int rateCharge = 0;
            //命中判定
            float hitRate = sourceUnitNumericCom.GetAsFloat(NumericType.HitRate) - destUnitNumericCom.GetAsFloat(NumericType.DodgeRate);
            rateCharge = RandomHelper.RandomNumber(0, 100);
            if (rateCharge / 100.0f > hitRate)
            {
                Game.EventSystem.Run(EventIdType.AttackMissing, destUnitId);
                Log.Debug("Miss!  命中率 "+ hitRate);
                return false;
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

            if (skillDamageValue.damageType != DamageType.Physic)
            {
                resistType = NumericType.MagicResist;

            }
            skillDamageValue.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * ( 100.0f / (destUnitNumericCom.GetAsInt(resistType) + 100.0f)));

            //预防可能要考虑什么白字红字,黑字粉字等乱七八糟的情况,所以专门用一个List
            DamageData[] array = new DamageData[1];
            array[0] = skillDamageValue;

            //计算最终伤害加成,减免

            for (int i = 0; i < array.Length; i++)
            {
                var damage = array[i];

                float finalDamagePct = 1 + sourceUnitNumericCom.GetAsFloat(NumericType.FinalDamage_AddPct) - destUnitNumericCom.GetAsFloat(NumericType.FinalDamage_ReducePct);

                damage.damageValue = Mathf.RoundToInt(array[i].damageValue * finalDamagePct);
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
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
            return false;
        }
    }

    public static void CalDotDamage(long sourceUnitId,  Unit destUnit, Buff_DOT dot)
    {
        DamageData damageData = new DamageData();
        damageData.damageType = dot.damageType;

        damageData.damageValue =  BuffHandlerVar.cacheDatas_int[(sourceUnitId,dot.buffSignal)];
        NumericComponent destUnitNumericCom = destUnit.GetComponent<NumericComponent>();
        NumericType resistType = NumericType.ArmorResist;

        if (dot.damageType != DamageType.Physic)
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

