using ETModel;
using UnityEngine;
using System;
// Code Generate By Tool : 2019/4/3 15:25:34

namespace ETHotfix
{

	[UIFactory(UIType.UILogin)]

	public class UILoginFactory : IUIFactory
	{
		private UILoginAdapterComponent adapter;

		public UI Create()
		{
			try
			{
				ResourcesComponent res = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
				res.LoadBundle(UIType.UILogin.StringToAB());
				GameObject bundleGameObject = res.GetAsset(UIType.UILogin.StringToAB(), UIType.UILogin) as GameObject;
				GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
				go.layer = LayerMask.NameToLayer(LayerNames.UI);
				UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UILogin, go ,false);
				var com = ui.AddComponent<UILoginComponent>();
				adapter = ui.AddComponent<UILoginAdapterComponent>();
				adapter.Init(com);
				return ui;
			}
			catch (Exception e)
			{
				Log.Error(e);
				return null;
			}
		}

		public void Start()
		{
			adapter.Start();
		}

		public ETTask OnEnable()
		{
			return adapter.OnEnable();
		}

		public ETTask OnDisable()
		{
			return adapter.OnDisable();
		}
	}
}
