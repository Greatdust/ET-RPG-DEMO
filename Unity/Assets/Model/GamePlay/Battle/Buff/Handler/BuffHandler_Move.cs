using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.Move)]
public class BuffHandler_Move : BaseBuffHandler, IBuffActionWithSetOutputHandler
{

    public IBuffReturnedValue ActionHandle(BaseBuffData data, Unit source, Unit target)
    {
        Buff_Move buff_Move = data as Buff_Move;



        Vector3 targetPos = target.Position + Vector3.Normalize(source.Position - target.Position) * buff_Move.targetPosOffset;
        BuffReturnedValue_MoveData returnedValue = new BuffReturnedValue_MoveData()
        {
            startPos = source.Position,
            endPos = targetPos,
             startDir = source.Rotation
        };

        if (buff_Move.resetDir)
            source.GameObject.transform.forward = target.Position - source.Position;
        if (buff_Move.flash || buff_Move.moveDuration == 0)
        {
            //瞬移
            source.Position = targetPos;
        }
        else
        {
            if (!string.IsNullOrEmpty(buff_Move.animatorBoolValue))
            {
                float moveSpeed = Vector3.Distance(targetPos, source.Position) / buff_Move.moveDuration;
                AnimatorComponent animatorComponent = source.GetComponent<AnimatorComponent>();
                animatorComponent.SetBoolValue(buff_Move.animatorBoolValue, true);
                animatorComponent.SetAnimatorSpeed(moveSpeed / 10);//TODO: 默认单位移动速度10m/s,以后要根据配置表来
            }
            //CharacterCtrComponent characterCtrComponent = source.GetComponent<CharacterCtrComponent>();
            //characterCtrComponent.MoveToAsync(targetPos, buff_Move.moveDuration, () =>
            // {
            //     if (!string.IsNullOrEmpty(buff_Move.animatorBoolValue))
            //     {
            //         AnimatorComponent animatorComponent = source.GetComponent<AnimatorComponent>();
            //         animatorComponent.SetBoolValue(buff_Move.animatorBoolValue, false);
            //         animatorComponent.SetAnimatorSpeed(1);
            //     }
            // }, null).Coroutine();
        }
        return returnedValue;
    }

    public IBuffReturnedValue ActionHandle(BaseBuffData data, Unit source, List<IBuffReturnedValue> baseBuffReturnedValues)
    {

           BuffReturnedValue_TargetUnit? buffReturnedValue_TargetUnit = baseBuffReturnedValues[0] as BuffReturnedValue_TargetUnit?;
            Unit target = buffReturnedValue_TargetUnit.Value.target;
        return ActionHandle(data, source, target);
    }
}



