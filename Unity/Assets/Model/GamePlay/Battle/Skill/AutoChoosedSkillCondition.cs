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
    ActiveSkillData GetActiveSkillData(UnitActionData data);
}

public class ChooseSkill_单体伤害类 : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get; set; } = true;
    public int Priority { get; set; } = 3;

    public ActiveSkillData GetActiveSkillData(UnitActionData data)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(data)) return null;
        ActiveSkillComponent activeSkillComponent = data.mUnit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.isNormalAttack) continue;
            if (v.activeSkillTag == ActiveSkillTag.单体伤害类)
            {
                if (SkillHelper.CheckActiveConditions(v, data.mUnit))
                {
                    return v;
                }
            }
        }
        return null;
    }

    public bool MeetCondition(UnitActionData data)
    {
        return true; 
    }
}

public class ChooseSkill_群体伤害类 : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get; set; } = true;
    public int Priority { get; set; } = 2;
    public int targetNum = 2;
    public bool MeetCondition(UnitActionData data)
    {
        BattleMgrComponent battleMgr = BattleMgrComponent.Instance;
        if (data.mTeam == UnitTeam.Enemy)
        {
            if (battleMgr.BattleData.playerTeam.Count >= targetNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (battleMgr.BattleData.enemyTeam.Count >= targetNum)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
    public ActiveSkillData GetActiveSkillData(UnitActionData data)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(data)) return null;
        ActiveSkillComponent activeSkillComponent = data.mUnit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.activeSkillTag == ActiveSkillTag.群体伤害类)
            {
                if (SkillHelper.CheckActiveConditions(v, data.mUnit))
                {
                    return v;
                }
            }
        }
        return null;
    }
}

public class ChooseSkill_单体Buff类 : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get ; set ; }
    public int Priority { get; set; } = 1;

    public int timeSpan = 5;
    public bool MeetCondition(UnitActionData data)
    {
        TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(this.GetHashCode());
        if (timer.remainTime <= 0)
        {
            TimeSpanHelper.Timing(timer, timeSpan);
            return true;
        }
        else
            return false;
    }
    public ActiveSkillData GetActiveSkillData(UnitActionData data)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(data)) return null;
        ActiveSkillComponent activeSkillComponent = data.mUnit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.activeSkillTag == ActiveSkillTag.单体BUFF类)
            {
                if (SkillHelper.CheckActiveConditions(v, data.mUnit))
                {
                    return v;
                }
            }
        }
        return null;
    }
}

public class ChooseSkill_群体Buff类 : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get ; set ; } = true;
    public int Priority { get; set; } = 0;
    public int targetNum = 2;
    public int timeSpan = 5;
    public bool MeetCondition(UnitActionData data)
    {
        BattleMgrComponent battleMgr = BattleMgrComponent.Instance;
        bool targetNumEnough = false;
        if (data.mTeam == UnitTeam.Enemy)
        {
            if (battleMgr.BattleData.enemyTeam.Count >= targetNum)
            {
                targetNumEnough = true;
            }
            else
            {
                return false;
            }
        }
        else
        {
            if (battleMgr.BattleData.playerTeam.Count >= targetNum)
            {
                targetNumEnough = true;
            }
            else
            {
                return false;
            }
        }
        if (targetNumEnough)
        {
            TimeSpanHelper.Timer timer = TimeSpanHelper.GetTimer(this.GetHashCode());
            if (timer.remainTime <= 0)
            {
                TimeSpanHelper.Timing(timer, timeSpan);
                return true;
            }
            else
                return false;
        }
        return false;
    }
    public ActiveSkillData GetActiveSkillData(UnitActionData data)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(data)) return null;
        ActiveSkillComponent activeSkillComponent = data.mUnit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.activeSkillTag == ActiveSkillTag.群体BUFF类)
            {
                if (SkillHelper.CheckActiveConditions(v, data.mUnit))
                {
                    return v;
                }
            }
        }
        return null;
    }
}

public class ChooseSkill_单体治疗类 : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get; set; } = true;
    public int Priority { get; set; } = 4;
    public float hpPct = 0.6f;
    public bool MeetCondition(UnitActionData data)
    {      
        return true;
    }
    public ActiveSkillData GetActiveSkillData(UnitActionData data)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(data)) return null;
        ActiveSkillComponent activeSkillComponent = data.mUnit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.activeSkillTag == ActiveSkillTag.单体治疗类)
            {
                if (SkillHelper.CheckActiveConditions(v, data.mUnit))
                {
                    BattleMgrComponent battleMgr = BattleMgrComponent.Instance;
                    if (data.mTeam == UnitTeam.Enemy)
                    {
                        foreach (var unit in battleMgr.BattleData.enemyTeam)
                        {
                            NumericComponent numericComponent = unit.mUnit.GetComponent<NumericComponent>();
                            if (numericComponent.GetAsFloat(NumericType.气血剩余百分比) <= hpPct)
                            {
                                foreach (var buff in v.AllBuffInSkill)
                                {
                                    if (buff.Value.TargetType != BuffTargetType.对我方单体) continue;
                                    if (buff.Value.buffData.GetBuffIdType() == BuffIdType.GiveRecover
                                        || buff.Value.buffData.GetBuffIdType() == BuffIdType.AddBuff)
                                    {
                                        v.buffReturnValues.TryGetValue(buff.Value.buffSignal, out var buffReturnedValues);
                                        if (buffReturnedValues == null)
                                        {
                                            buffReturnedValues = new List<IBuffReturnedValue>();
                                            v.buffReturnValues[buff.Value.buffSignal] = buffReturnedValues;
                                        }
                                        buffReturnedValues.Add(new BuffReturnedValue_TargetUnit() { target = unit.mUnit , playSpeedScale = buff.Value.ParentSkillData.skillExcuteSpeed});
                                    }
                                }
                                return v;
                            }
                        }
                    }
                    else
                    {
                        foreach (var unit in battleMgr.BattleData.playerTeam)
                        {
                            NumericComponent numericComponent = unit.mUnit.GetComponent<NumericComponent>();
                            if (numericComponent.GetAsFloat(NumericType.气血剩余百分比) <= hpPct)
                            {
                                foreach (var buff in v.AllBuffInSkill)
                                {
                                    if (buff.Value.TargetType != BuffTargetType.对我方单体) continue;
                                    if (buff.Value.buffData.GetBuffIdType() == BuffIdType.GiveRecover
                                        || buff.Value.buffData.GetBuffIdType() == BuffIdType.AddBuff)
                                    {
                                        v.buffReturnValues.TryGetValue(buff.Value.buffSignal, out var buffReturnedValues);
                                        if (buffReturnedValues == null)
                                        {
                                            buffReturnedValues = new List<IBuffReturnedValue>();
                                            v.buffReturnValues[buff.Value.buffSignal] = buffReturnedValues;
                                        }
                                        buffReturnedValues.Add(new BuffReturnedValue_TargetUnit() { target = unit.mUnit, playSpeedScale = buff.Value.ParentSkillData.skillExcuteSpeed });
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

public class ChooseSkill_群体治疗类 : IAutoChoosedSkillCondition
{
    public bool NeedCheck { get; set; } = true;
    public int Priority { get; set; } = 5;
    public int targetNum = 2;
    public float hpPct = 0.5f;
    public bool MeetCondition(UnitActionData data)
    {
        BattleMgrComponent battleMgr = BattleMgrComponent.Instance;
        int count = 0;
        if (data.mTeam == UnitTeam.Enemy)
        {
            foreach (var v in battleMgr.BattleData.enemyTeam)
            {
                NumericComponent numericComponent = v.mUnit.GetComponent<NumericComponent>();
                if (numericComponent.GetAsFloat(NumericType.气血剩余百分比) <= hpPct)
                {
                    count++;
                }
            }
        }
        else
        {
            foreach (var v in battleMgr.BattleData.playerTeam)
            {
                NumericComponent numericComponent = v.mUnit.GetComponent<NumericComponent>();
                if (numericComponent.GetAsFloat(NumericType.气血剩余百分比) <= hpPct)
                {
                    count++;
                }
            }
        }
        if (count >= targetNum)
            return true;
        else
            return false;
    }

    public ActiveSkillData GetActiveSkillData(UnitActionData data)
    {
        if (!NeedCheck) return null;
        if (!MeetCondition(data)) return null;
        ActiveSkillComponent activeSkillComponent = data.mUnit.GetComponent<ActiveSkillComponent>();
        foreach (var v in activeSkillComponent.activeSkillDic.Values)
        {
            if (v.activeSkillTag == ActiveSkillTag.群体治疗类)
            {
                if (SkillHelper.CheckActiveConditions(v, data.mUnit))
                {
                    BattleMgrComponent battleMgr = BattleMgrComponent.Instance;
                    if (data.mTeam == UnitTeam.Enemy)
                    {
                        foreach (var unit in battleMgr.BattleData.enemyTeam)
                        {
                            NumericComponent numericComponent = unit.mUnit.GetComponent<NumericComponent>();
                            if (numericComponent.GetAsFloat(NumericType.气血剩余百分比) <= hpPct)
                            {
                                foreach (var buff in v.AllBuffInSkill)
                                {
                                    if (buff.Value.TargetType != BuffTargetType.对我方全体) continue;
                                    if (buff.Value.buffData.GetBuffIdType() == BuffIdType.GiveRecover
                                        || buff.Value.buffData.GetBuffIdType() == BuffIdType.AddBuff)
                                    {
                                        v.buffReturnValues.TryGetValue(buff.Value.buffSignal, out var buffReturnedValues);
                                        if (buffReturnedValues == null)
                                        {
                                            buffReturnedValues = new List<IBuffReturnedValue>();
                                            v.buffReturnValues[buff.Value.buffSignal] = buffReturnedValues;
                                        }
                                        buffReturnedValues.Add(new BuffReturnedValue_TargetUnit() { target = unit.mUnit, playSpeedScale = buff.Value.ParentSkillData.skillExcuteSpeed });
                                    }
                                }
                                return v;
                            }
                        }
                    }
                    else
                    {
                        foreach (var unit in battleMgr.BattleData.playerTeam)
                        {
                            NumericComponent numericComponent = unit.mUnit.GetComponent<NumericComponent>();
                            if (numericComponent.GetAsFloat(NumericType.气血剩余百分比) <= hpPct)
                            {
                                foreach (var buff in v.AllBuffInSkill)
                                {
                                    if (buff.Value.TargetType != BuffTargetType.对我方全体) continue;
                                    if (buff.Value.buffData.GetBuffIdType() == BuffIdType.GiveRecover
                                        || buff.Value.buffData.GetBuffIdType() == BuffIdType.AddBuff)
                                    {
                                        v.buffReturnValues.TryGetValue(buff.Value.buffSignal, out var buffReturnedValues);
                                        if (buffReturnedValues == null)
                                        {
                                            buffReturnedValues = new List<IBuffReturnedValue>();
                                            v.buffReturnValues[buff.Value.buffSignal] = buffReturnedValues;
                                        }
                                        buffReturnedValues.Add(new BuffReturnedValue_TargetUnit() { target = unit.mUnit, playSpeedScale = buff.Value.ParentSkillData.skillExcuteSpeed });
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
