#if UNITY_EDITOR
using ETModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class BattleTest_Init : MonoBehaviour
{
    private void Start()
    {
        Time.fixedDeltaTime = 1f / 60;
        this.StartAsync();
    }

    private void StartAsync()
    {
        try
        {
            SynchronizationContext.SetSynchronizationContext(OneThreadSynchronizationContext.Instance);

            DontDestroyOnLoad(gameObject);
            Game.EventSystem.Add(DLLType.Model, typeof(Init).Assembly);

            Game.Scene.AddComponent<TimerComponent>();
            Game.Scene.AddComponent<GlobalConfigComponent>();

            Game.Scene.AddComponent<ResourcesComponent>();
            Game.Scene.AddComponent<PlayerComponent>();
            Game.Scene.AddComponent<UnitComponent>();
            Game.Scene.AddComponent<UIComponent>();
            Game.Scene.AddComponent<EmitObjUnitComponent>();
            Game.Scene.AddComponent<AudioMgrComponent, AudioPlayerMgr>(GetComponent<AudioPlayerMgr>());

            Game.Scene.AddComponent<NumericWatcherComponent>();
            Game.Scene.AddComponent<CommandSimulaterComponent>();

            // 加载配置
            Game.Scene.GetComponent<ResourcesComponent>().LoadBundle("config.unity3d");
            Game.Scene.AddComponent<ConfigComponent>();
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle("config.unity3d");

            Game.Scene.AddComponent<BattleTestComponent>();


        }
        catch (Exception e)
        {
            Log.Error(e);
        }
    }

    private void Update()
    {
        OneThreadSynchronizationContext.Instance.Update();
        Game.Hotfix.Update?.Invoke();
        Game.EventSystem.Update();
    }

    private void FixedUpdate()
    {
        Game.Hotfix.FixedUpdate?.Invoke();
        Game.EventSystem.FixedUpdate();
    }

    private void LateUpdate()
    {
        Game.Hotfix.LateUpdate?.Invoke();
        Game.EventSystem.LateUpdate();
    }

    private void OnApplicationQuit()
    {
        Game.Hotfix.OnApplicationQuit?.Invoke();
        Game.Close();
    }
}
#endif
