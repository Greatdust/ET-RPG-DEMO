using ETModel;
using UnityEngine;
using System;
// Code Generate By Tool : 2019/4/3 15:34:30

namespace ETHotfix
{

	[UIFactory(UIType.UILobby)]

	public class UILobbyFactory : IUIFactory
	{
		private UILobbyAdapterComponent adapter;

		public UI Create()
		{
			try
			{
				ResourcesComponent res = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
				res.LoadBundle(UIType.UILobby.StringToAB());
				GameObject bundleGameObject = res.GetAsset(UIType.UILobby.StringToAB(), UIType.UILobby) as GameObject;
				GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
				go.layer = LayerMask.NameToLayer(LayerNames.UI);
				UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UILobby, go ,false);
				var com = ui.AddComponent<UILobbyComponent>();
				adapter = ui.AddComponent<UILobbyAdapterComponent>();
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
