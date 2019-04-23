using Box2DSharp.Collision.Shapes;
using ETModel;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace ETHotfix
{
    public static class UnitFactory
    {
        private static Dictionary<string, PBoxData> boxDataDic = new Dictionary<string, PBoxData>();
        private static Dictionary<string, PCircleData> circleDataDic = new Dictionary<string, PCircleData>();

        public static Unit Create(long id,int typeId, UnitData unitData)
        {
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            UnitConfig unitConfig = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), typeId) as UnitConfig;

            unit.AddComponent<NumericComponent,int>(typeId);
            unit.AddComponent<UnitStateComponent>();
            unit.AddComponent<UnitPathComponent>();
            unit.AddComponent<BuffMgrComponent>();
            var activeSkillCom =  unit.AddComponent<ActiveSkillComponent>();
            var passiveSkillCom = unit.AddComponent<PassiveSkillComponent>();
            unit.AddComponent<SkillEffectComponent>();

            //添加碰撞体
            AddCollider(unit, unitData, true, unitConfig.AssetName);
            unit.AddComponent<CharacterStateComponent>();
            unit.AddComponent<CharacterMoveComponent>();
            unit.AddComponent<CalNumericComponent>();

        
            if (unitConfig.Skills != null && unitConfig.Skills.Length > 0)
            {
                SkillConfigComponent skillConfigComponent = Game.Scene.GetComponent<SkillConfigComponent>();
                foreach (var v in unitConfig.Skills)
                {
                    if (string.IsNullOrEmpty(v)) continue;
                    var activeSkill = skillConfigComponent.GetActiveSkill(v);
                    if (activeSkill != null)
                    {
                        Log.Debug(string.Format("{0} 添加主动技能 {1} ({2})成功!", typeId, v, activeSkill.skillName));
                        activeSkillCom.AddSkill(v);
                        continue;
                    }
                    var passiveSkill = skillConfigComponent.GetPassiveSkill(v);
                    if (passiveSkill != null)
                    {
                        Log.Debug(string.Format("{0} 添加被动技能 {1} ({2})成功!", typeId, v, passiveSkill.skillName));
                        passiveSkillCom.AddSkill(v);
                        continue;
                    }
                    Log.Error(v + "  这样的技能不存在!");
                }
            }

            //unit.AddComponent<TurnComponent>();

            unitComponent.Add(unit);
            return unit;
        }

        public static Unit CreateEmitObj(long id, UnitData unitData,string key)
        {
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
            Unit unit = ComponentFactory.CreateWithId<Unit>(id);
            unit.AddComponent<UnitStateComponent>();
            AddCollider(unit, unitData, true, key);
            unit.AddComponent<EmitObjMoveComponent>();

            return unit;
        }

        public static Unit CreateStaticObj_Box(UnitData unitData,PBoxData pBoxData)
        {
            Unit unit = ComponentFactory.CreateWithId<Unit>(IdGenerater.GenerateId());
            AddCollider_BoxData(unit, unitData, false, pBoxData,true);
            return unit;
        }

        static void AddCollider(Unit unit, UnitData unitData,bool isSensor,string key)
        {
            var configs = Game.Scene.GetComponent<ConfigComponent>().GetAll(typeof(NonStaticBodyConfig));
            if (boxDataDic.ContainsKey(key))
            {
                AddCollider_BoxData(unit, unitData, isSensor, boxDataDic[key]);
                return;
            }
            if (circleDataDic.ContainsKey(key))
            {
                AddCollider_CircleData(unit, unitData, isSensor, circleDataDic[key]);
                return;
            }

            foreach (var v in configs)
            {
                NonStaticBodyConfig nonStatic = v as NonStaticBodyConfig;
                if (nonStatic.Key == key)
                {
                    if (nonStatic.FilePath.Contains("BoxsData"))
                    {
                        var dic = DeserializeBox(nonStatic.FilePath);
                        foreach (var box in dic)
                        {
                            boxDataDic[box.Key] = box.Value;
                        }
                        AddCollider_BoxData(unit, unitData, isSensor, boxDataDic[key]);
                        return;
                    }
                    else if(nonStatic.FilePath.Contains("CirclesData"))
                    {
                        var dic = DeserializeCircle(nonStatic.FilePath);
                        foreach (var box in dic)
                        {
                            circleDataDic[box.Key] = box.Value;
                        }
                        AddCollider_CircleData(unit, unitData, isSensor, circleDataDic[key]);

                        return;
                    }
                }
            }

        }

        static Dictionary<string, PBoxData> DeserializeBox(string filePath)
        {
            Dictionary<string, PBoxData> data = MessagePack.MessagePackSerializer.Deserialize<Dictionary<string, PBoxData>>(File.ReadAllBytes(filePath),
          MessagePack.Resolvers.ContractlessStandardResolver.Instance);

            return data;
        }
        static Dictionary<string, PCircleData> DeserializeCircle(string filePath)
        {
            try
            {
                Dictionary<string, PCircleData> data = MessagePack.MessagePackSerializer.Deserialize<Dictionary<string, PCircleData>>(File.ReadAllBytes(filePath),
            MessagePack.Resolvers.ContractlessStandardResolver.Instance);

                return data;
            }
            catch (Exception e)
            {
                Log.Error(e.ToString());
                return null;
            }
        }

        public static void AddCollider_BoxData(Unit unit, UnitData unitData, bool isSensor, PBoxData pBoxData,bool usePos = false)
        {
            PBoxShape pBoxShape = new PBoxShape()
            {
                eulerAnglesY = pBoxData.eulerAnglesY,
                bodyType = pBoxData.bodyType,
                unitData = unitData,
                isSensor = isSensor
            };
            pBoxShape.offset = pBoxData.offset.ToV3();
            pBoxShape.size = pBoxData.size.ToV3();
            if (usePos)
            {
                unit.Position = pBoxData.pos.ToV3();
            }

            unit.AddComponent<P2DBodyComponent, PBaseShape>(pBoxShape);
        }
        public static void AddCollider_CircleData(Unit unit, UnitData unitData, bool isSensor, PCircleData pCircleData)
        {
            PCircleShape pCircleShape = new PCircleShape()
            {
                eulerAnglesY = pCircleData.eulerAnglesY,
                bodyType = pCircleData.bodyType,
                unitData = unitData,
                isSensor = isSensor
            };
            pCircleShape.offset = pCircleData.offset.ToV3();
            pCircleShape.radius = pCircleData.radius;
            pCircleShape.halfHeight = pCircleData.halfHeight;


            unit.AddComponent<P2DBodyComponent, PBaseShape>(pCircleShape);
        }
    }
}