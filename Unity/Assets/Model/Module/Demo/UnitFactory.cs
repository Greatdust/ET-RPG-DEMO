using Box2DSharp.Collision.Shapes;
using UnityEngine;

namespace ETModel
{
    public static class UnitFactory
    {
        public static Unit Create(long id,int typeId)
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
            //unit.AddComponent<TurnComponent>();

            unitComponent.Add(unit);
            return unit;
        }

        public static Unit CreateEmitObj(GameObject go,UnitLayer unitLayer,UnitLayerMask unitLayerMask, Shape shape)
        {
            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();
            Unit unit = ComponentFactory.CreateWithId<Unit,GameObject>(IdGenerater.GenerateId(), go);
            unit.UnitLayer = unitLayer;
            unit.LayerMask = unitLayerMask;

            unit.AddComponent<EmitObjMoveComponent>();
            unit.AddComponent<PDynamicBodyComponent, Shape>(shape);
            unitComponent.Add(unit);
            return unit;
        }
    }
}