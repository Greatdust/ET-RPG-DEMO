using ETModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

public static class SceneChangeHelper
{
    public static async void SceneChange(string sceneId, Action beforeActiveAction, Action afterActiveAction)
    {
        SceneChangeComponent sceneChangeComponent = Game.Scene.AddComponent<SceneChangeComponent>();
        AudioMgrComponent audioMgrComponent = Game.Scene.GetComponent<AudioMgrComponent>();
        audioMgrComponent.PauseBg();
       // UIComponent ui = Game.Scene.GetComponent<UIComponent>();
       // ui.();
       // UI loadingUI = await ui.Create(UIType.UILoading);
        SceneManager.LoadScene("LoadingScene");
        await sceneChangeComponent.ChangeSceneAsync(sceneId);
       // loadingUI.GetComponent<UILoadingAdapterComponent>().SetProgress(1);
        if (beforeActiveAction != null)
        {
            beforeActiveAction();
        }
      //  ui.Remove(UIType.UILoading);
       // ui.DisplayAllUI();
        sceneChangeComponent.loadMapOperation.allowSceneActivation = true;
        if (afterActiveAction != null)
        {
            afterActiveAction();
        }
       // audioMgrComponent.PlayBg(Game.Scene.GetComponent<SceneComponent>().GetScene(sceneId).sceneBg);
        Game.Scene.RemoveComponent<SceneChangeComponent>();
    }
}
