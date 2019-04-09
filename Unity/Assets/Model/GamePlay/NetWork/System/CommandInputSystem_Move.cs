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
            UnitPathComponent unitPathComponent = unit.GetComponent<UnitPathComponent>();

            PathfindingComponent pathfindingComponent = Game.Scene.GetComponent<PathfindingComponent>();
            unitPathComponent.ABPath = ComponentFactory.Create<ABPathWrap, Vector3, Vector3>(unit.Position, input_Move.clickPos);
            pathfindingComponent.Search(unitPathComponent.ABPath);

            CommandResult_Move result_Move = CommandGCHelper.GetCommandResult<CommandResult_Move>();

            result_Move.Path = unitPathComponent.ABPath.Result;
            // result_Move.dir = input_Move.moveDir;// 暂时就以输入的方向作为角色的方向
            return result_Move;

        }
    }
}
