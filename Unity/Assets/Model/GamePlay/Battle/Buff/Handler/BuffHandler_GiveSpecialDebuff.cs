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

    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_GiveSpecialDebuff buff = data as Buff_GiveSpecialDebuff;

        if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到
        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            UnitStateComponent unitState = target.GetComponent<UnitStateComponent>();
            //从一个特殊效果配置中,拿到对应效果的BuffGroup,添加到角色的BuffMgrComponent中
            switch (buff.restrictionType)
            {
                case RestrictionType.击退:
                case RestrictionType.眩晕:
                    Property_NotInControl property_NotInControl = unitState.GetCurrState<Property_NotInControl>();
                    property_NotInControl.Set(true);
                    break;
            }

        }
    }

    public void Remove(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        Buff_GiveSpecialDebuff buff = data as Buff_GiveSpecialDebuff;

        if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到
        foreach (var v in baseBuffReturnedValues)
        {
            BufferValue_TargetUnits? buffReturnedValue_TargetUnit = v as BufferValue_TargetUnits?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
            UnitStateComponent unitState = target.GetComponent<UnitStateComponent>();
            //从一个特殊效果配置中,拿到对应效果的BuffGroup,添加到角色的BuffMgrComponent中
            switch (buff.restrictionType)
            {
                case RestrictionType.击退:
                case RestrictionType.眩晕:
                    Property_NotInControl property_NotInControl = unitState.GetCurrState<Property_NotInControl>();
                    property_NotInControl.Set(false);
                    break;
            }

        }
    }
}



