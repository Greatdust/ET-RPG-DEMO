

using ETModel;

[Event(EventIdType.GiveDamage)]
public class Unit_GiveDamageEvent : AEvent<long, GameCalNumericTool.DamageData[]>
{
    public override void Run(long a, GameCalNumericTool.DamageData[] b)
    {
        var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();

        calNumeric.GetDamage(b);
        M2C_GiveDamage m2C_GiveDamage = new M2C_GiveDamage();
        m2C_GiveDamage.Id = a;
        foreach (var v in b)
        {
            m2C_GiveDamage.DamageDatas.Add(new DamageData()
            {
                DamageType = (int)v.damageType,
                DamageValue = v.damageValue
            });
            Log.Debug("造成伤害  " + v.damageValue);
        }
        ETHotfix.MessageHelper.Broadcast(m2C_GiveDamage);
    }
}

[Event(EventIdType.AttackMissing)]
public class Unit_AttackMissingEvent : AEvent<long,long>
{
    public override void Run(long a, long b)
    {

        M2C_GiveAttackMissing m2c = new M2C_GiveAttackMissing();
        m2c.Id = b;
        ETHotfix.MessageHelper.Broadcast(m2c);
    }
}

[Event(EventIdType.OnUnitDie)]
public class Unit_GetDieEvent : AEvent<long>
{
    public override void Run(long a)
    {
        var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();

        calNumeric.GetDie();

    }
}

[Event(EventIdType.GiveHealth)]
public class Unit_GetHealthEvent : AEvent<long, int>
{
    public override void Run(long a, int b)
    {
        var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();

        calNumeric.GetHP(b);
        M2C_GiveHealth m2c = new M2C_GiveHealth();
        m2c.Id = a;
        m2c.HealthValue = b;
        ETHotfix.MessageHelper.Broadcast(m2c);
    }
}

[Event(EventIdType.GiveMp)]
public class Unit_GetMPEvent : AEvent<long, int>
{
    public override void Run(long a, int b)
    {
        var calNumeric = UnitComponent.Instance.Get(a).GetComponent<CalNumericComponent>();

        calNumeric.GetMP(b);

    }
}


