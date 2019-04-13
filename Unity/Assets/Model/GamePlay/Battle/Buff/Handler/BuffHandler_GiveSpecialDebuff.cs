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
    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
        Buff_GiveSpecialDebuff buff = (Buff_GiveSpecialDebuff)buffHandlerVar.data;
        if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到
        BufferValue_TargetUnits buffReturnedValue_TargetUnit = (BufferValue_TargetUnits)buffHandlerVar.bufferValues[typeof(BufferValue_TargetUnits)];
        foreach (var v in buffReturnedValue_TargetUnit.targets)
        {
            UnitStateComponent unitState = v.GetComponent<UnitStateComponent>();
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


    public void Remove(BuffHandlerVar buffHandlerVar)
    {
        Buff_GiveSpecialDebuff buff = (Buff_GiveSpecialDebuff)buffHandlerVar.data;
        if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到
        BufferValue_TargetUnits buffReturnedValue_TargetUnit = (BufferValue_TargetUnits)buffHandlerVar.bufferValues[typeof(BufferValue_TargetUnits)];
        foreach (var v in buffReturnedValue_TargetUnit.targets)
        {
            UnitStateComponent unitState = v.GetComponent<UnitStateComponent>();
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



