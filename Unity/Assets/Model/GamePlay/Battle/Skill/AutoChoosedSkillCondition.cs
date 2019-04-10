using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public interface IAutoChoosedSkillCondition
{
    bool NeedCheck { get; set; }
    int Priority { get; set; }
    ActiveSkillData GetActiveSkillData(Unit unit);
}

public class ChooseSkill_Damage : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get; set; } = true;
    public int Priority { get; set; } = 3;

    public ActiveSkillData GetActiveSkillData(Unit unit)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(unit)) return null;
        ActiveSkillComponent activeSkillComponent = unit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.isNormalAttack) continue;
            if (v.activeSkillTag == ActiveSkillTag.Damage)
            {
                if (SkillHelper.CheckActiveConditions(v, unit))
                {
                    return v;
                }
            }
        }
        return null;
    }

    public bool MeetCondition(Unit unit)
    {
        return true; 
    }
}


public class ChooseSkill_Buff : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get ; set ; }
    public int Priority { get; set; } = 1;

    public int timeSpan = 5;
    public bool MeetCondition(Unit data)
    {
        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(this.GetHashCode());
        if (timer.interval <= 0)
        {
            TimeSpanHelper.Timing(timer, timeSpan);
            return true;
        }
        else
            return false;
    }
    public ActiveSkillData GetActiveSkillData(Unit unit)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(unit)) return null;
        ActiveSkillComponent activeSkillComponent = unit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.activeSkillTag == ActiveSkillTag.Buff)
            {
                if (SkillHelper.CheckActiveConditions(v, unit))
                {
                    return v;
                }
            }
        }
        return null;
    }
}


public class ChooseSkill_Restore : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get; set; } = true;
    public int Priority { get; set; } = 4;
    public float hpPct = 0.6f; // 因为这里的自动选择技能时给怪物或者NPC准备的,它们一般不会有MP恢复这个概念,所以这里只考虑的HP
    public bool MeetCondition(Unit data)
    {      
        return true;
    }
    public ActiveSkillData GetActiveSkillData(Unit unit)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(unit)) return null;
        ActiveSkillComponent activeSkillComponent = unit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.activeSkillTag == ActiveSkillTag.Restore)
            {
                if (SkillHelper.CheckActiveConditions(v, unit))
                {

                    DungeonComponent dungeon = Game.Scene.GetComponent<DungeonComponent>();
                    if (unit.UnitTeam == UnitTeam.Enemy)
                    {
                        foreach (var u in dungeon.enemyTeam)
                        {
                            NumericComponent numericComponent = u.GetComponent<NumericComponent>();
                            if (numericComponent.GetAsFloat(NumericType.HP_RemainPct) <= hpPct)
                            {
                                foreach (var buff in v.AllBuffInSkill)
                                {
                                    if (buff.Value.TargetType != BuffTargetType.范围内我方角色 && buff.Value.TargetType!= BuffTargetType.自身) continue;
                                    if (buff.Value.buffData.GetBuffIdType() == BuffIdType.GiveRecover
                                        || buff.Value.buffData.GetBuffIdType() == BuffIdType.AddBuff)
                                    {
                                        v.buffReturnValues.TryGetValue(buff.Value.buffSignal, out var buffReturnedValues);
                                        if (buffReturnedValues == null)
                                        {
                                            buffReturnedValues = new List<IBuffReturnedValue>();
                                            v.buffReturnValues[buff.Value.buffSignal] = buffReturnedValues;
                                        }
                                        buffReturnedValues.Add(new BuffReturnedValue_TargetUnit() { target = u , playSpeedScale = buff.Value.ParentSkillData.skillExcuteSpeed});
                                    }
                                }
                                return v;
                            }
                        }
                    }
                    else
                    {
                        foreach (var u in dungeon.playerTeam)
                        {
                            NumericComponent numericComponent = u.GetComponent<NumericComponent>();
                            if (numericComponent.GetAsFloat(NumericType.HP_RemainPct) <= hpPct)
                            {
                                foreach (var buff in v.AllBuffInSkill)
                                {
                                    if (buff.Value.TargetType != BuffTargetType.范围内我方角色 && buff.Value.TargetType != BuffTargetType.自身) continue;
                                    if (buff.Value.buffData.GetBuffIdType() == BuffIdType.GiveRecover
                                        || buff.Value.buffData.GetBuffIdType() == BuffIdType.AddBuff)
                                    {
                                        v.buffReturnValues.TryGetValue(buff.Value.buffSignal, out var buffReturnedValues);
                                        if (buffReturnedValues == null)
                                        {
                                            buffReturnedValues = new List<IBuffReturnedValue>();
                                            v.buffReturnValues[buff.Value.buffSignal] = buffReturnedValues;
                                        }
                                        buffReturnedValues.Add(new BuffReturnedValue_TargetUnit() { target = u, playSpeedScale = buff.Value.ParentSkillData.skillExcuteSpeed });
                                    }
                                }
                                return v;
                            }
                        }
                    }
                }
            }
        }
        return null;
    }
}