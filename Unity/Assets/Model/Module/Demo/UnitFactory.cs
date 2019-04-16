using Box2DSharp.Collision.Shapes;
using UnityEngine;

namespace ETModel
{
    public static class UnitFactory
    {
        public static Unit Create(long id,int typeId, UnitData unitData)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();

            UnitConfig unitConfig = Game.Scene.GetComponent<ConfigComponent>().Get(typeof(UnitConfig), typeId) as UnitConfig;
            resourcesComponent.LoadBundle(unitConfig.ABPacketName.ToLower().StringToAB());

            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset(unitConfig.ABPacketName.ToLower().StringToAB(), unitConfig.ABPacketName);
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();

            GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
            Unit unit = ComponentFactory.CreateWithId<Unit, GameObject>(id, go);

            unit.AddComponent<NumericComponent,int>(typeId);
            unit.AddComponent<AnimatorComponent>();
            unit.AddComponent<UnitStateComponent>();
            unit.AddComponent<CharacterMoveComponent>();
            unit.AddComponent<UnitPathComponent>();
            unit.AddComponent<AudioComponent>();
            unit.AddComponent<BuffMgrComponent>();
            var activeSkillCom =  unit.AddComponent<ActiveSkillComponent>();
            var passiveSkillCom = unit.AddComponent<PassiveSkillComponent>();
            unit.AddComponent<SkillEffectComponent>();

            //添加碰撞体
            AddCollider(unit, unitData, true);
            unit.AddComponent<CharacterStateComponent>();
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

        public static Unit CreateEmitObj(GameObject go, UnitData unitData)
        {
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
            Unit unit = ComponentFactory.CreateWithId<Unit,GameObject>(IdGenerater.GenerateId(), go);
            AddCollider(unit, unitData, true);
            unit.AddComponent<EmitObjMoveComponent>();
            
            unitComponent.Add(unit);
            return unit;
        }
        public static void AddCollider(Unit unit, UnitData unitData,bool isSensor)
        {
            PBaseColliderHelper pBaseColliderHelper = unit.GameObject.GetComponentInChildren<PBaseColliderHelper>();
            PBaseShape pBaseShape = null;


            switch (pBaseColliderHelper)
            {
                case PBoxColliderHelper pBoxCollider:
                    PBoxShape pBoxShape = new PBoxShape()
                    {
                        eulerAnglesY = pBoxCollider.transform.eulerAngles.y,
                        offset = pBoxCollider.offset,
                        size = pBoxCollider.size,
                        bodyType = pBoxCollider.bodyType,
                        unitData = unitData,
                        isSensor = isSensor
                    };

                    pBaseShape = pBoxShape;
                    break;
                case PCircleColliderHelper pCircleCollider:
                    PCircleShape pCircleShape = new PCircleShape()
                    {
                        eulerAnglesY = pCircleCollider.transform.eulerAngles.y,
                        offset = pCircleCollider.offset,
                        radius = pCircleCollider.radius,
                        halfHeight = pCircleCollider.height,
                        bodyType = pCircleCollider.bodyType,
                        unitData = unitData,
                        isSensor = isSensor
                    };
                    pBaseShape = pCircleShape;
                    break;
            }
            var bodyCom = unit.AddComponent<PDynamicBodyComponent, PBaseShape>(pBaseShape);
            pBaseColliderHelper.bodyComponent = bodyCom;
        }
    }
}