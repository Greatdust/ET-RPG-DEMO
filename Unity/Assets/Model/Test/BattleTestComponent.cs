#if UNITY_EDITOR
using Box2DSharp.Collision.Shapes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ETModel
{
    [ObjectSystem]
    public class BattleTestComponentAwakeSystem : AwakeSystem<BattleTestComponent>
    {
        public override void Awake(BattleTestComponent self)
        {
            self.Awake();
        }
    }

    [ObjectSystem]
    public class BattleTestComponentFixedUpdateSystem : FixedUpdateSystem<BattleTestComponent>
    {
        public override void FixedUpdate(BattleTestComponent self)
        {
            self.FixedUpdate();
        }
    }


    public class BattleTestComponent :Component
    {
        public List<Unit> otherPlayers;

        public void Awake()
        {
            otherPlayers = new List<Unit>();
            Game.Scene.GetComponent<GlobalConfigComponent>().networkPlayMode = false;
            Game.EventSystem.Run(EventIdType.LoadAssets);
            {
                UnitData playerData = new UnitData();
                playerData.groupIndex = GroupIndex.Player;
                playerData.layerMask = UnitLayerMask.ALL;
                playerData.unitLayer = UnitLayer.Character;
                playerData.unitTag = UnitTag.Player;
                //创建主角
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, playerData);
                v.Position = new UnityEngine.Vector3(-10, 0, -10);
                UnitComponent.Instance.MyUnit = v;
                v.AddComponent<CameraComponent>();
                v.AddComponent<CommandComponent>();
                var input = v.AddComponent<InputComponent>();
                var list = v.GetComponent<ActiveSkillComponent>().skillList.Keys.ToArray();
                input.AddSkillToHotKey("Q", list[0]);
                input.AddSkillToHotKey("W", list[1]);
                input.AddSkillToHotKey("E", list[2]);
                BattleEventHandler.LoadAssets(v);
            }
            UnitData monsterData = new UnitData();
            monsterData.groupIndex = GroupIndex.Monster;
            monsterData.layerMask = UnitLayerMask.ALL;
            monsterData.unitLayer = UnitLayer.Character;
            monsterData.unitTag = UnitTag.Monster;
            {
                //创建怪物
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, monsterData);
                v.Position = new UnityEngine.Vector3(4.2f, 4, -15);
                v.Rotation = UnityEngine.Quaternion.LookRotation(v.Position - UnitComponent.Instance.MyUnit.Position, Vector3.up);
                //v.AddComponent<PDynamicBodyComponent, Shape>(new CircleShape() { Radius = 0.5f });

                BattleEventHandler.LoadAssets(v);
                otherPlayers.Add(v);
            }
            {
                //创建怪物
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, monsterData);
                v.Position = new UnityEngine.Vector3(-10, 0, 11);
                v.Rotation = UnityEngine.Quaternion.LookRotation(v.Position - UnitComponent.Instance.MyUnit.Position, Vector3.up);
                //v.AddComponent<PDynamicBodyComponent, Shape>(new CircleShape() { Radius = 0.5f });

                BattleEventHandler.LoadAssets(v);
                otherPlayers.Add(v);
            }
            {
                //创建怪物
                Unit v = UnitFactory.Create(IdGenerater.GenerateId(), 1001, monsterData);
                v.Position = new UnityEngine.Vector3(-10, 0, 15);
                v.Rotation = UnityEngine.Quaternion.LookRotation(v.Position - UnitComponent.Instance.MyUnit.Position, Vector3.up);
                //v.AddComponent<PDynamicBodyComponent, Shape>(new CircleShape() { Radius = 0.5f });

                BattleEventHandler.LoadAssets(v);
                otherPlayers.Add(v);
            }

            //这里准备其他角色施放技能的参数
        }

        void GetInput(Unit unit, Pipeline_WaitForInput pipeline_WaitForInput)
        {
            if (!SkillHelper.tempData.ContainsKey((unit, pipeline_WaitForInput.pipelineSignal)))
            {
                SkillHelper.tempData[(unit, pipeline_WaitForInput.pipelineSignal)] = new Dictionary<Type, IBufferValue>();
            }
   
            switch (pipeline_WaitForInput.inputType)
            {

                //等待用户输入,可能有正确输入/取消/输入超时三种情况

                case InputType.Tar:
                    //SkillHelper.tempData[(unit, pipeline_WaitForInput.pipelineSignal)][typeof(BufferValue_TargetUnits)] = ;

                    break;
                case InputType.Dir:
                    
                    break;
                case InputType.Pos:
                    Log.Debug("获得输入的位置");
                    SkillHelper.tempData[(unit, pipeline_WaitForInput.pipelineSignal)][typeof(BufferValue_Pos)] = new BufferValue_Pos() { aimPos = unit.Position };
                    break;
                case InputType.Charge:
                    break;
                case InputType.Spell:

                    break;
                case InputType.ContinualSpell:

 
                    break;


            }
        }

        public void FixedUpdate()
        {
            foreach (var v in otherPlayers)
            {
                var list = v.GetComponent<ActiveSkillComponent>().skillList.Keys.ToArray();
                if (!SkillHelper.CheckIfSkillCanUse(list[1], v)) continue;
                var activeSkillData = Game.Scene.GetComponent<SkillConfigComponent>().GetActiveSkill(list[1]);
                Pipeline_WaitForInput pipeline_WaitForInput = activeSkillData.inputCheck.Find(p => p.GetTriggerType() == Pipeline_TriggerType.等待用户输入) as Pipeline_WaitForInput;
                if (pipeline_WaitForInput != null)
                    GetInput(v, pipeline_WaitForInput);
                v.GetComponent<ActiveSkillComponent>().Execute(list[1]).Coroutine();
            }
        }
    }
}
#endif
