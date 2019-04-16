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

    public void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        Buff_PlayAnim buff_PlayAnim = (Buff_PlayAnim)buffHandlerVar.data;

        if (buff_PlayAnim.playSpeed == 0)
            buff_PlayAnim.playSpeed = 1;

        if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits bufferValue_TargetUnits))
        {
            Log.Error("找不到对应的目标!");
            return;
        }
        foreach (var v in bufferValue_TargetUnits.targets)
        {
            //角色动画
            AnimatorComponent animatorComponent = v.GetComponent<AnimatorComponent>();
            if (!string.IsNullOrEmpty(buff_PlayAnim.anim_boolValue))
            {
                animatorComponent.SetBoolValue(buff_PlayAnim.anim_boolValue, buff_PlayAnim.boolValue);
            }
            if (!string.IsNullOrEmpty(buff_PlayAnim.anim_triggerValue))
            {
                animatorComponent.SetTrigger(buff_PlayAnim.anim_triggerValue);
            }
            
            float speed = buffHandlerVar.playSpeed * buff_PlayAnim.playSpeed;

           // animatorComponent.SetAnimatorSpeed(speed);
        }

#endif
    }
}



