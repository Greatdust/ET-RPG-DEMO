using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

[BuffType(BuffIdType.GiveSpecialDebuff)]
public class BuffHandler_GiveSpecialDebuff : BaseBuffHandler,IBuffActionWithGetInputHandler,IBuffRemoveHanlder
{

    public void ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        Buff_GiveSpecialDebuff buff = data as Buff_GiveSpecialDebuff;

        if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到
        foreach (var v in baseBuffReturnedValues)
        {
            BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = v as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            BuffMgrComponent buffMgrComponent = target.GetComponent<BuffMgrComponent>();
            CharacterStateComponent characterStateComponent = target.GetComponent<CharacterStateComponent>();
            //从一个特殊效果配置中,拿到对应效果的BuffGroup,添加到角色的BuffMgrComponent中
            switch (buff.restrictionType)
            {
                case RestrictionType.灵封:
                    characterStateComponent.CanUseActiveSkill = false;
                    break;
                case RestrictionType.冰封:
                    characterStateComponent.CanDoAction = false;
                    break;
                case RestrictionType.眩晕:
                    characterStateComponent.CanDoAction = false;
                    break;
            }

        }
    }

    public void Remove(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {
        Buff_GiveSpecialDebuff buff = data as Buff_GiveSpecialDebuff;

        if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到
        foreach (var v in baseBuffReturnedValues)
        {
            BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = v as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            BuffMgrComponent buffMgrComponent = target.GetComponent<BuffMgrComponent>();
            CharacterStateComponent characterStateComponent = target.GetComponent<CharacterStateComponent>();
            //从一个特殊效果配置中,拿到对应效果的BuffGroup,添加到角色的BuffMgrComponent中
            switch (buff.restrictionType)
            {
                case RestrictionType.灵封:
                    characterStateComponent.CanUseActiveSkill = true;
                    break;
                case RestrictionType.冰封:
                    characterStateComponent.CanDoAction = true;
                    break;
                case RestrictionType.眩晕:
                    characterStateComponent.CanDoAction = true;
                    break;
            }

        }
    }
}



