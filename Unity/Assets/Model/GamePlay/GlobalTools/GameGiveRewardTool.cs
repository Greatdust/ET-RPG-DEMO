using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public static class GameGiveRewardTool
{
   

    public static void GiveExp(Unit unit,int num,out float preProgress,out float currProgress,out bool lvUp)
    {
        NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
        if (numericComponent.GetAsInt(NumericType.Level) == numericComponent.GetAsInt(NumericType.LevelMax))
        {
            preProgress = 1;
            currProgress = 1;
            lvUp = false;
            return;
        }
        Debug.LogFormat("单位{0}获得经验 {1}", unit.Id, num);
        int expMax = numericComponent.GetAsInt(NumericType.ExpMax);
        int preExp = numericComponent.GetAsInt(NumericType.Exp);
        preProgress = preExp / ((float)expMax);
        int currExp = preExp + num;
        lvUp = false;
        if (currExp >= expMax)
        {
            numericComponent.Set(NumericType.Level, numericComponent.GetAsInt(NumericType.Level) + 1);
            lvUp = true;
            Game.EventSystem.Run(EventIdType.UnitLvUp, unit.Id, numericComponent.GetAsInt(NumericType.Level));
            if (numericComponent.GetAsInt(NumericType.Level) == numericComponent.GetAsInt(NumericType.LevelMax))
            {
                currProgress = 1;
                return;
            }
            currExp -= expMax;
            ConfigComponent configComponent = Game.Scene.GetComponent<ConfigComponent>();
            //ExpForLevelUp expConfig = configComponent.Get(typeof(ExpForLevelUp), numericComponent.GetAsInt(NumericType.等级)) as ExpForLevelUp;
            //expMax = expConfig.Exp;
            //numericComponent.Set(NumericType.经验Max, expMax);
        }
        currProgress = Mathf.Clamp01(currExp / ((float)expMax));
        numericComponent.Set(NumericType.Exp, currExp);

    }

    public static void GiveNumeric(NumericType numericType, float value)
    {
        Unit mUnit = UnitComponent.Instance.MyUnit;
        NumericComponent numericComponent = mUnit.GetComponent<NumericComponent>();
        numericComponent.Set(numericType, numericComponent.GetAsFloat(numericType) + value);
    }
}

