using ETModel;
using System;
using System.Collections.Generic;
using System.Text;

namespace ETHotfix
{
    [ObjectSystem]
    public class NumericComponentAwakeSystem : AwakeSystem<NumericComponent, int>
    {
        public override void Awake(NumericComponent self, int typeId)
        {
            self.Awake(typeId);
        }
    }

    public static class NumericComponentSystem
    {
        public static void Awake(this NumericComponent self,int typeId)
        {
            // 这里初始化base值
            UnitConfig unitConfig = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), typeId) as UnitConfig;

            self.Set(NumericType.ArmorResist, unitConfig.ArmorResist);
            self.Set(NumericType.ATK, unitConfig.ATK);
            self.Set(NumericType.CritDamagePct, unitConfig.CritDamagePct);
            self.Set(NumericType.CritRate, unitConfig.CritRate);
            self.Set(NumericType.DodgeRate, unitConfig.DodgeRate);


            self.Set(NumericType.HitRate, unitConfig.HitRate);
            self.Set(NumericType.HP, unitConfig.HPMax);
            self.Set(NumericType.HPMax_Base, unitConfig.HPMax);


            self.Set(NumericType.HP_LeechRate, unitConfig.HP_LeechRate);
            self.Set(NumericType.HP_Restore, unitConfig.HP_Restore);
            self.Set(NumericType.Level, unitConfig.Level);
            self.Set(NumericType.MagicATK, unitConfig.MagicATK);
            self.Set(NumericType.MagicResist, unitConfig.MagicResist);

            self.Set(NumericType.MoveSpeed, unitConfig.MoveSpeed);
            self.Set(NumericType.MP, unitConfig.MPMax);
            self.Set(NumericType.MPMax_Base, unitConfig.MPMax);
            self.Set(NumericType.MP_LeechRate, unitConfig.MP_LeechRate);
            self.Set(NumericType.MP_Restore, unitConfig.MP_Restore);
        }
    }
}
