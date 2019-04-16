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

        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }

        //if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到

        foreach (var v in targetUnits.targets)
        {
            CharacterStateComponent unitState = v.GetComponent<CharacterStateComponent>();
            //从一个特殊效果配置中,拿到对应效果的BuffGroup,添加到角色的BuffMgrComponent中
            switch (buff.restrictionType)
            {
                case RestrictionType.击退:
                    unitState.Set(SpecialStateType.NotInControl, true);
                    break;
                case RestrictionType.眩晕:
                    unitState.Set(SpecialStateType.CantDoAction,true);
                    break;
            }
        }
    }


    public void Remove(BuffHandlerVar buffHandlerVar)
    {
        Buff_GiveSpecialDebuff buff = (Buff_GiveSpecialDebuff)buffHandlerVar.data;
        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits targetUnits))
        {
            return;
        }
        //if (buff.currStackNum < buff.aimStackNum) return;//叠加层数没达到

        foreach (var v in targetUnits.targets)
        {
            CharacterStateComponent unitState = v.GetComponent<CharacterStateComponent>();
            //从一个特殊效果配置中,拿到对应效果的BuffGroup,添加到角色的BuffMgrComponent中
            switch (buff.restrictionType)
            {
                case RestrictionType.击退:
                    unitState.Set(SpecialStateType.NotInControl, false);
                    break;
                case RestrictionType.眩晕:
                    unitState.Set(SpecialStateType.CantDoAction, false);
                    break;
            }
        }
    }
}



