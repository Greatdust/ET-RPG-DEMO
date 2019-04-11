using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

[BuffType(BuffIdType.MoveBack)]
public class BuffHandler_MoveBack : BaseBuffHandler, IBuffActionWithGetInputHandler
{

    public void ActionHandle(IBuffData data, Unit source, List<IBufferValue> baseBuffReturnedValue)
    {
        
        BufferValue_Pos? returnedValue = baseBuffReturnedValue[0] as BufferValue_Pos?;
        foreach (var v in baseBuffReturnedValue)
        {
            returnedValue = v as BufferValue_Pos?;
            if (returnedValue!=null)
            {
                break;
            }
        }
        Buff_MoveBack buff_MoveBack = data as Buff_MoveBack;
        if (buff_MoveBack.resetDir)
            source.GameObject.transform.forward = returnedValue.Value.startPos - source.Position;
        if (buff_MoveBack.flash || buff_MoveBack.moveDuration == 0)
        {
            //瞬移
            source.Position = returnedValue.Value.startPos;
            source.Rotation = returnedValue.Value.startDir;
        }
        else
        {
            //CharacterCtrComponent characterCtrComponent = source.GetComponent<CharacterCtrComponent>();
            //characterCtrComponent.MoveToAsync(returnedValue.Value.startPos, buff_MoveBack.moveDuration, () =>
            //{
            //    source.Rotation = returnedValue.Value.startDir;
            //}, null).Coroutine();
        }
    }
}



