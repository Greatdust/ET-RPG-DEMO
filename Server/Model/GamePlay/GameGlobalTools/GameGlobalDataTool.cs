using ETModel;
using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class GameGlobalDataTool
{

    #region Item,Equip,Exp,Numeric...etc

   
    [Serializable]
    public struct ExpDataForReward
    {
        public int exp;
    }
    [Serializable]
    public struct NumericDataForReward
    {
        public NumericType numericType;
        public float value;
    }

    [Serializable]
    public class HPRecoverData
    {
        public float pct;
        public float value;
        public void Excute(long unitId)
        {
            if (pct > 0)
            {
                Unit unit = UnitComponent.Instance.MyUnit;
                float maxValue = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.HPMax_Final);
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, unitId, maxValue * pct);
            }
            if (value > 0)
            {

                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.HP, unitId, value);
            }
        }
    }

    [Serializable]
    public class MPRecoverData
    {
        public float pct;
        public float value;
        public void Excute(long unitId)
        {
            if (pct > 0)
            {
                Unit unit = UnitComponent.Instance.MyUnit;
                float maxValue = unit.GetComponent<NumericComponent>().GetAsFloat(NumericType.HPMax_Final);
                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, unitId, maxValue * pct);
            }
            if (value > 0)
            {

                Game.EventSystem.Run(EventIdType.NumbericChange, NumericType.MP, unitId, value);
            }
        }
    }

    #endregion
}




