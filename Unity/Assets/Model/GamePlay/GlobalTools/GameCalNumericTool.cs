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
        无属性伤害,
        金属性伤害,
        木属性伤害,
        水属性伤害,
        火属性伤害,
        土属性伤害
    }
    public class DamageData
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
            UnitActionData unitActionData = BattleMgrComponent.Instance.unitActionDataDic[destUnitId];
            if (unitActionData.die) return;

            NumericComponent sourceUnitNumericCom = UnitComponent.Instance.Get(sourceUnitId).GetComponent<NumericComponent>();
            NumericComponent destUnitNumericCom = UnitComponent.Instance.Get(destUnitId).GetComponent<NumericComponent>();
            int rateCharge = 0;
            //命中判定
            float hitRate = sourceUnitNumericCom.GetAsFloat(NumericType.命中率) - destUnitNumericCom.GetAsFloat(NumericType.闪避率);
            rateCharge = RandomHelper.RandomNumber(0, 100);
            if (rateCharge / 100.0f > hitRate)
            {
                Game.EventSystem.Run(EventIdType.AttackMissing, sourceUnitId, destUnitId);
                return;
            }

            //暴击判定
            float criticalRate = sourceUnitNumericCom.GetAsFloat(NumericType.暴击率);
            rateCharge = RandomHelper.RandomNumber(0, 100);
            if (rateCharge / 100.0f <= criticalRate)
            {
                //暴击判定通过
                skillDamageValue.isCritical = true;
                skillDamageValue.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * sourceUnitNumericCom.GetAsFloat(NumericType.暴击伤害));
            }

            //计算属性亲和/抗性后的伤害
            if (skillDamageValue.damageType != DamageType.无属性伤害)
            {
                NumericType qinHe = NumericType.金属性亲和;
                NumericType kangXing = NumericType.五行抗性;
                switch (skillDamageValue.damageType)
                {
                    case DamageType.金属性伤害:
                        qinHe = NumericType.金属性亲和;
                        break;
                    case DamageType.木属性伤害:
                        qinHe = NumericType.木属性亲和;
                        break;
                    case DamageType.水属性伤害:
                        qinHe = NumericType.水属性亲和;
                        break;
                    case DamageType.火属性伤害:
                        qinHe = NumericType.火属性亲和;
                        break;
                    case DamageType.土属性伤害:
                        qinHe = NumericType.土属性亲和;
                        break;
                }
                skillDamageValue.damageValue = Mathf.RoundToInt(
                    (skillDamageValue.damageValue * (1 + sourceUnitNumericCom.GetAsInt(qinHe)))
                    * (1 - 100 / (destUnitNumericCom.GetAsFloat(kangXing) + 100))
                    );
            }

            //计算护体,各属性伤害附加率后的伤害
            skillDamageValue.damageValue -= destUnitNumericCom.GetAsInt(NumericType.护体Final);

            List<DamageData> damageList = new List<DamageData>();
            damageList.Add(skillDamageValue);
            if (skillDamageValue.damageType == DamageType.无属性伤害)
            {
                float damageAddRate = sourceUnitNumericCom.GetAsFloat(NumericType.金属性伤害附加率);
                if (damageAddRate > 0)
                {
                    DamageData damageData = new DamageData();
                    damageData.damageType = DamageType.金属性伤害;
                    damageData.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * damageAddRate);
                    damageData.isCritical = false;
                    damageList.Add(damageData);
                }
                damageAddRate = sourceUnitNumericCom.GetAsFloat(NumericType.木属性伤害附加率);
                if (damageAddRate > 0)
                {
                    DamageData damageData = new DamageData();
                    damageData.damageType = DamageType.木属性伤害;
                    damageData.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * damageAddRate);
                    damageData.isCritical = false;
                    damageList.Add(damageData);
                }
                damageAddRate = sourceUnitNumericCom.GetAsFloat(NumericType.水属性伤害附加率);
                if (damageAddRate > 0)
                {
                    DamageData damageData = new DamageData();
                    damageData.damageType = DamageType.水属性伤害;
                    damageData.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * damageAddRate);
                    damageData.isCritical = false;
                    damageList.Add(damageData);
                }
                damageAddRate = sourceUnitNumericCom.GetAsFloat(NumericType.火属性伤害附加率);
                if (damageAddRate > 0)
                {
                    DamageData damageData = new DamageData();
                    damageData.damageType = DamageType.火属性伤害;
                    damageData.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * damageAddRate);
                    damageData.isCritical = false;
                    damageList.Add(damageData);
                }
                damageAddRate = sourceUnitNumericCom.GetAsFloat(NumericType.土属性伤害附加率);
                if (damageAddRate > 0)
                {
                    DamageData damageData = new DamageData();
                    damageData.damageType = DamageType.土属性伤害;
                    damageData.damageValue = Mathf.RoundToInt(skillDamageValue.damageValue * damageAddRate);
                    damageData.isCritical = false;
                    damageList.Add(damageData);
                }

            }

            //判断是否是防御姿态
            CharacterStateComponent characterStateComponent = UnitComponent.Instance.Get(destUnitId).GetComponent<CharacterStateComponent>();

            //计算最终伤害加成,减免

            for (int i = 0; i < damageList.Count; i++)
            {
                damageList[i].damageValue = Mathf.RoundToInt(damageList[i].damageValue *
                    (1 + sourceUnitNumericCom.GetAsFloat(NumericType.最终伤害加成率) - destUnitNumericCom.GetAsFloat(NumericType.最终伤害减免率)));
                if (characterStateComponent.DefensiveStance)
                {
                    damageList[i].damageValue =Mathf.RoundToInt(0.8f * damageList[i].damageValue);//防御姿态减少20%伤害.
                }
                //限定最小伤害1
                damageList[i].damageValue = Mathf.Clamp(damageList[i].damageValue, 1, int.MaxValue);
            }


            //给予伤害
            Game.EventSystem.Run(EventIdType.GiveDamage, destUnitId, damageList);

            //给予吸血,吸法
            float xiQu = sourceUnitNumericCom.GetAsFloat(NumericType.气血吸取率);
            if (xiQu > 0)
                Game.EventSystem.Run(EventIdType.GiveHealth, sourceUnitId, Mathf.RoundToInt(skillDamageValue.damageValue * xiQu *
                    (1 + sourceUnitNumericCom.GetAsFloat(NumericType.受到恢复效果加成率))));
            xiQu = sourceUnitNumericCom.GetAsFloat(NumericType.法力吸取率);
            if (xiQu > 0)
                Game.EventSystem.Run(EventIdType.GiveMp, sourceUnitId, skillDamageValue.damageValue * xiQu);
        }
        catch (Exception e)
        {
            Debug.LogError(e.ToString());
        }
    }

    public static void CalDotDamage(Unit destUnit, Buff_DOT dot)
    {
        DamageData damageData = new DamageData();
        damageData.damageType = dot.damageType;
        damageData.damageValue = dot.damageValue;
        NumericComponent destUnitNumericCom = destUnit.GetComponent<NumericComponent>();
        if (damageData.damageType != DamageType.无属性伤害)
        {
            damageData.damageValue = Mathf.RoundToInt(
                (damageData.damageValue * (1 + dot.qinHe))
                * (1 - 100 / (destUnitNumericCom.GetAsFloat( NumericType.五行抗性) + 100))
                );
        }

        damageData.damageValue -= destUnitNumericCom.GetAsInt(NumericType.护体Final);
        damageData.damageValue = Mathf.RoundToInt(damageData.damageValue *
               (1 + dot.damageFinalAddPct - destUnitNumericCom.GetAsFloat(NumericType.最终伤害减免率)));

        List<DamageData> damageList = new List<DamageData>();
        damageList.Add(damageData);
        Game.EventSystem.Run(EventIdType.GiveDamage, destUnit.Id, NumericType.HP, damageList);
    }

    public static void CalRestore(Unit sourceUnit)
    {
        NumericComponent unitNumericCom = sourceUnit.GetComponent<NumericComponent>();
        int hp = unitNumericCom.GetAsInt(NumericType.气血每秒回复);
        if (hp <= 0)
        {
            return;
        }
        Game.EventSystem.Run(EventIdType.GiveHealth, sourceUnit.Id, Mathf.RoundToInt( hp * (1 + unitNumericCom.GetAsFloat(NumericType.受到恢复效果加成率))));
        int mp = unitNumericCom.GetAsInt(NumericType.法力每秒回复);
        if (mp <= 0)
        {
            return;
        }
        Game.EventSystem.Run(EventIdType.GiveMp, sourceUnit.Id, mp);
    }
}

