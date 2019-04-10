using Sirenix.OdinInspector;

namespace ETModel
{
    public enum NumericRealtionType
    {
        [LabelText(">")]
        Greater,
        [LabelText("<")]
        Less,
        [LabelText("=")]
        Equal,
        [LabelText("!=")]
        NotEqual,
        [LabelText(">=")]
        GreaterEqual,
        [LabelText("<=")]
        LessEqual
    }

    public enum NumericType
    {
        Max = 10000,
        #region 数值基本类型定义
        //一般一个属性最多有以下几部分组成,以Property为例
        //Property代表玩家当前属性.计算公式为x=(x_base+x_add)*(1+x_Pct)
        //PropertyBase代表玩家这个属性的基础值
        //PropertyAdd代表玩家这个属性的直接增加值
        //PropertyPct代表玩家这个属性的百分比增加值
        Level = 1000,
        LevelMax = 1001,


        Exp = 1002,
        ExpMax = 1003,

        [LabelText("魔抗")]
        MagicResist = 1019,//魔抗
        [LabelText("护甲")]
        ArmorResist = 1020,// 护甲

        HitRate = 1021,
        DodgeRate = 1022,
        Speed = 1023,
        CritRate = 1024,//暴击率
        CritDamagePct = 1025,//暴击伤害(百分比)
        [LabelText("吸血率")]
        HP_LeechRate = 1028,//造成伤害的百分之多少,转化为自身HP
        [LabelText("吸蓝率")]
        MP_LeechRate = 1029,//造成伤害的百分之多少,转化为自身MP

        [LabelText("HP每秒恢复")]
        HP_Restore = 1031,
        [LabelText("MP每秒恢复")]
        MP_Restore = 1032,


        FinalDamage_ReducePct = 1040,
        FinalDamage_AddPct = 1041,





        #endregion
        #region 数值附加类型定义


        HP_Final = 5001,
        HPMax_Final = 5002,
        HPMax_Base = HPMax_Final * 10 + 1,
        HPMax_Add,
        HPMax_Pct,
        HP_LoseValue,//气血减少值
        HP_LosePct,//气血减少百分比
        HP_RemainPct,//气血剩余百分比

        MP = 5003,
        MPMax_Final = 5004,
        MPMax_Base = MPMax_Final * 10 + 1,
        MPMax_Add,
        MPMax_Pct,
        MP_LoseValue,//法力减少值
        MP_LosePct,//法力减少百分比
        MP_RemainPct,//法力剩余百分比

        

        #endregion
    }
}
