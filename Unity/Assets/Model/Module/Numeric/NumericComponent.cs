using System;
using System.Collections.Generic;

namespace ETModel
{
	[ObjectSystem]
	public class NumericComponentAwakeSystem : AwakeSystem<NumericComponent,int>
	{
		public override void Awake(NumericComponent self, int typeId)
		{
			self.Awake(typeId);
		}
	}

	public class NumericComponent: Component
	{
		public readonly Dictionary<int, int> NumericDic = new Dictionary<int, int>();

		public void Awake(int typeId)
		{
            // 这里初始化base值
            UnitConfig unitConfig = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), typeId) as UnitConfig;

            Set(NumericType.ArmorResist, unitConfig.ArmorResist);
            Set(NumericType.ATK, unitConfig.ATK);
            Set(NumericType.CritDamagePct, unitConfig.CritDamagePct);
            Set(NumericType.CritRate, unitConfig.CritRate);
            Set(NumericType.DodgeRate, unitConfig.DodgeRate);

 
            Set(NumericType.HitRate, unitConfig.HitRate);
            Set(NumericType.HP, unitConfig.HPMax);
            Set(NumericType.HPMax_Base, unitConfig.HPMax);


            Set(NumericType.HP_LeechRate, unitConfig.HP_LeechRate);
            Set(NumericType.HP_Restore, unitConfig.HP_Restore);
            Set(NumericType.Level, unitConfig.Level);
            Set(NumericType.MagicATK, unitConfig.MagicATK);
            Set(NumericType.MagicResist, unitConfig.MagicResist);

            Set(NumericType.MoveSpeed, unitConfig.MoveSpeed);
            Set(NumericType.MP, unitConfig.MPMax);
            Set(NumericType.MPMax_Base, unitConfig.MPMax);
            Set(NumericType.MP_LeechRate, unitConfig.MP_LeechRate);
            Set(NumericType.MP_Restore, unitConfig.MP_Restore);

        }

		public float GetAsFloat(NumericType numericType)
		{
			return (float)GetByKey((int)numericType) / 10000;
		}

		public int GetAsInt(NumericType numericType)
		{
			return GetByKey((int)numericType);
		}

		public void Set(NumericType nt, float value)
		{
			this[nt] = (int) (value * 10000);
		}

		public void Set(NumericType nt, int value)
		{
			this[nt] = value;
		}

		public int this[NumericType numericType]
		{
			get
			{
				return this.GetByKey((int) numericType);
			}
			set
			{
				int v = this.GetByKey((int) numericType);
				if (v == value)
				{
					return;
				}

				NumericDic[(int)numericType] = value;

				Update(numericType);
			}
		}

		private int GetByKey(int key)
		{
			int value = 0;
			this.NumericDic.TryGetValue(key, out value);
			return value;
		}

		public void Update(NumericType numericType)
		{
			if (numericType < NumericType.Max)
			{
				return;
			}
			int final = (int) numericType / 10;
			int bas = final * 10 + 1; 
			int add = final * 10 + 2;
			int pct = final * 10 + 3;


			// 一个数值可能会多种情况影响，比如速度,加个buff可能增加速度绝对值100，也有些buff增加10%速度，所以一个值可以由5个值进行控制其最终结果
			// final = (((base + add) * (100 + pct) / 100) + finalAdd) * (100 + finalPct) / 100;
			this.NumericDic[final] = ((this.GetByKey(bas) + this.GetByKey(add)) * (100 + this.GetByKey(pct))) / 100;
			Game.EventSystem.Run(EventIdType.NumericUpdated, this.Entity.Id, (NumericType) final, final);

            int reduceValue = final * 10 + 4;
            if (!Enum.IsDefined(typeof(NumericType), reduceValue)) return;

            int reducePct = final * 10 + 5;
            int remainPct = final * 10 + 6;

            Set((NumericType)reduceValue, this.GetAsFloat((NumericType)final) - this.GetAsFloat((NumericType)(final - 1)));
            Set((NumericType)reducePct, this.GetAsFloat((NumericType)reduceValue)/this.GetAsFloat((NumericType)final));
            Set((NumericType)remainPct, 1 - this.GetAsFloat((NumericType)reducePct));

        }
    }
}