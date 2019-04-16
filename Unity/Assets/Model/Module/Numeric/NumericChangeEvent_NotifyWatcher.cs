using UnityEngine;

namespace ETModel
{
	// 分发数值监听
	[Event(EventIdType.NumbericChange)]
	public class NumericChangeEvent_NotifyWatcher: AEvent<NumericType, long, float>
	{
		public override void Run( NumericType nt, long destUnitId, float updateValue)
		{
            Unit unit = UnitComponent.Instance.Get(destUnitId);
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
      
            CharacterStateComponent unitStateComponent = unit.GetComponent<CharacterStateComponent>();
            if (!unitStateComponent.Get(SpecialStateType.Die))
                return ; //角色已经死亡,不再受到任何属性影响
            switch (nt)
            {
                case NumericType.HP:

                    int value = numericComponent.GetAsInt(NumericType.HP) + Mathf.RoundToInt(updateValue);
                    int maxValue = numericComponent.GetAsInt(NumericType.HPMax_Final);
                    value = Mathf.Clamp(value, 0, maxValue);
                    numericComponent.Set(NumericType.HP, value);
                    Game.EventSystem.Run(EventIdType.HPChanged, destUnitId);
                    if (value == 0)
                    {
                        Game.EventSystem.Run(EventIdType.OnUnitDie, destUnitId);
                    }
                    break;
                case NumericType.MP:
                    value = numericComponent.GetAsInt(NumericType.MP) + Mathf.RoundToInt(updateValue);
                    maxValue = numericComponent.GetAsInt(NumericType.MPMax_Final);
                    value = Mathf.Clamp(value, 0, maxValue);
                    numericComponent.Set(NumericType.MP, value);
                    Game.EventSystem.Run(EventIdType.MPChanged, destUnitId);
                    break;
                
                //case NumericType.HPMax_Pct:
                //case NumericType.HP_LeechRate:
                //case NumericType.MPMax_Pct:
                //case NumericType.MP_LeechRate:
                //case NumericType.HitRate:
                //case NumericType.CritDamagePct:
                //case NumericType.CritRate:
                //case NumericType.DodgeRate:
                //case NumericType.FinalDamage_AddPct:
                //case NumericType.FinalDamage_ReducePct:
                //    //这些全都是百分比的值(float)
                //    float fvalue = numericComponent.GetAsFloat(nt) + updateValue;
                //    numericComponent.Set(nt, fvalue);
                    break;
                case NumericType.HP_LosePct:
                case NumericType.HP_RemainPct:
                case NumericType.MP_LosePct:
                case NumericType.MP_RemainPct:
                case NumericType.HP_LoseValue:
                case NumericType.MP_LoseValue:
                    Log.Error("这些属性不应该被直接改变,它们是间接被改变的值");
                    break;
                default:
                    float fvalue = numericComponent.GetAsFloat(nt) + updateValue;
                    numericComponent.Set(nt, fvalue);
                    break;
            }
        }
	}
}
