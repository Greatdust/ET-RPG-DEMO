using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using PF;

[BuffType(BuffIdType.PlayAnim)]
public class BuffHandler_PlayAnim : BaseBuffHandler,IBuffActionWithGetInputHandler
{
    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValues)
    {
        BufferValue_TargetUnits? target = null;
        foreach (var v in baseBuffReturnedValues)
        {
            target = v as BufferValue_TargetUnits?;
            if (target != null)
            {
                break;
            }
        }

        Buff_PlayAnim buff_PlayAnim = data as Buff_PlayAnim;
        //角色动画
        AnimatorComponent animatorComponent = source.GetComponent<AnimatorComponent>();
        if (!string.IsNullOrEmpty(buff_PlayAnim.anim_boolValue))
        {
            animatorComponent.SetBoolValue(buff_PlayAnim.anim_boolValue, buff_PlayAnim.boolValue);
        }
        if (!string.IsNullOrEmpty(buff_PlayAnim.anim_triggerValue))
        {
            animatorComponent.SetTrigger(buff_PlayAnim.anim_triggerValue);
        }
        float speed = target.Value.playSpeedScale;
        if (buff_PlayAnim.playTime > 0)
        {
            speed = speed * (buff_PlayAnim.origin / buff_PlayAnim.playTime);

        }
        animatorComponent.SetAnimatorSpeed(speed);

    }
}



