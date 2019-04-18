using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.Move)]
public class BuffHandler_Move : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public async void ActionHandle(BuffHandlerVar buffHandlerVar)
    {
#if !SERVER
        if (Game.Scene.GetComponent<GlobalConfigComponent>().networkPlayMode)
        {
            //联网模式是服务器发消息,才执行
            return;
        }
#endif
        try
        {
            Buff_Move buff_Move = (Buff_Move)buffHandlerVar.data;
            if (!buffHandlerVar.GetBufferValue(out BufferValue_Pos bufferValue_Pos))
            {
                Log.Error("给移动的Buff提供的参数不包含目标位置!  " + buffHandlerVar.skillId);
                return;
            }
            if (!buffHandlerVar.GetBufferValue(out BufferValue_TargetUnits bufferValue_TargetUnits))
            {
                Log.Error("给移动的Buff提供的参数不包含移动的目标!  " + buffHandlerVar.skillId);
                return;
            }

            foreach (var v in bufferValue_TargetUnits.targets)
            {

                if (buff_Move.resetDir)
                    v.Rotation = Quaternion.LookRotation(bufferValue_Pos.aimPos - v.Position,Vector3.up);
                Vector3 aimPos = bufferValue_Pos.aimPos;

                //TODO: 下面的移动都不严谨, 要做位移的合法性检查

                if (buff_Move.flash || buff_Move.moveDuration == 0)
                {
                    //需要检查目标位置是否能瞬移过去,然后瞬移不过去的时候,找到最合理的一个点瞬移过去
                    //瞬移
                    v.Position = aimPos;
                }
                else
                {
                    //需要检查目标位置是否能移动过去,如果不行的话,就不位移了
                    CharacterMoveComponent characterMoveComponent = buffHandlerVar.source.GetComponent<CharacterMoveComponent>();
                    float moveSpeed = Vector3.Distance(v.Position, aimPos) / buff_Move.moveDuration;
                    await characterMoveComponent.MoveTo(aimPos, moveSpeed);


                }
            }
        }
        catch (Exception e)
        {
            Log.Error(e.ToString());
        }
    }
}



