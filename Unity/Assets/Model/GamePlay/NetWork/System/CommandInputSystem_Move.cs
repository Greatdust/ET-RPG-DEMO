using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PF;
using UnityEngine;

namespace ETModel
{
    [CommandInput(typeof(CommandInput_Move))]
    public class CommandInputSystem_Move: ICommandSimulater
    {

        public ICommandResult Simulate(ICommandInput commandInput, Unit unit)
        {
            CommandInput_Move input_Move = commandInput as CommandInput_Move;
            NumericComponent numericComponent = unit.GetComponent<NumericComponent>();
            UnitStateComponent unitStateComponent = unit.GetComponent<UnitStateComponent>();
            Property_Position property_Position = unitStateComponent.unitProperty[typeof(Property_Position)] as Property_Position;

            float moveSpeed = numericComponent.GetAsFloat(NumericType.Speed);

            Vector3 moveDelta =  (input_Move.moveDir.normalized) * moveSpeed * EventSystem.FixedUpdateTime;
            CommandResult_Move result_Move = CommandGCHelper.GetCommandResult<CommandResult_Move>();
            result_Move.postion = moveDelta + property_Position.Get();

            // result_Move.dir = input_Move.moveDir;// 暂时就以输入的方向作为角色的方向
            return result_Move;

        }
    }
}
