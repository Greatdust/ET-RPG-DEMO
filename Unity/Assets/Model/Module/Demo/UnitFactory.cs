using UnityEngine;

namespace ETModel
{
    public static class UnitFactory
    {
        public static Unit Create(long id)
        {
            ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
            GameObject bundleGameObject = (GameObject)resourcesComponent.GetAsset("Unit.unity3d", "Unit");
            GameObject prefab = bundleGameObject.Get<GameObject>("First");

            UnitComponent unitComponent = Game.Scene.GetComponent<UnitComponent>();

            GameObject go = UnityEngine.Object.Instantiate(prefab);
            Unit unit = ComponentFactory.CreateWithId<Unit, GameObject>(id, go);

            unit.AddComponent<NumericComponent>();
            unit.AddComponent<AnimatorComponent>();
            unit.AddComponent<CharacterCtrComponent>();
            unit.AddComponent<UnitStateComponent>();
            unit.AddComponent<TurnComponent>();

            unitComponent.Add(unit);
            return unit;
        }
    }
}