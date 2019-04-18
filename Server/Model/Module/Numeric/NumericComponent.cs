using System;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{

	public class NumericComponent: Component
	{
		public readonly Dictionary<int, float> NumericDic = new Dictionary<int, float>();


        public float GetAsFloat(NumericType numericType)
		{
			return (float)GetByKey((int)numericType);
		}

		public int GetAsInt(NumericType numericType)
		{
			return Mathf.RoundToInt(GetByKey((int)numericType));
		}

		public void Set(NumericType nt, float value)
		{
			this[nt] = value;
		}

		public void Set(NumericType nt, int value)
		{
			this[nt] = value;
		}

		public float this[NumericType numericType]
		{
			get
			{
				return this.GetByKey((int) numericType);
			}
			set
			{
				float v = this.GetByKey((int) numericType);

				NumericDic[(int)numericType] = value;

				Update(numericType);
			}
		}

		private float GetByKey(int key)
		{
			float value = 0;
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
            this.NumericDic[final] = ((this.GetByKey(bas) + this.GetByKey(add)) * (1 + this.GetByKey(pct)));
			Game.EventSystem.Run(EventIdType.NumericUpdated, this.Entity.Id, (NumericType) final, this.NumericDic[final]);

            int reduceValue = final * 10 + 4;
            //if (!Enum.TryParse<NumericType>(reduceValue.IntToString(),out var result)) return;

            //int reducePct = final * 10 + 5;
            //int remainPct = final * 10 + 6;

            //Set((NumericType)reduceValue, this.GetAsFloat((NumericType)final) - this.GetAsFloat((NumericType)(final - 1)));
            //Set((NumericType)reducePct, this.GetAsFloat((NumericType)reduceValue)/this.GetAsFloat((NumericType)final));
            //Set((NumericType)remainPct, 1 - this.GetAsFloat((NumericType)reducePct));

        }
    }
}