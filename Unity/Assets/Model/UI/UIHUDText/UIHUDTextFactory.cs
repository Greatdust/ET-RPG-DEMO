using ETModel;
using UnityEngine;
using System;
// Code Generate By Tool : 2019/4/16 21:43:05

namespace ETModel
{

	[UIFactory(UIType.UIHUDText)]

	public class UIHUDTextFactory : IUIFactory
	{
		private UIHUDTextAdapterComponent adapter;

		public UI Create()
		{
			try
			{
				ResourcesComponent res = ETModel.Game.Scene.GetComponent<ResourcesComponent>();
				res.LoadBundle(UIType.UIHUDText.StringToAB());
				GameObject bundleGameObject = res.GetAsset(UIType.UIHUDText.StringToAB(), UIType.UIHUDText) as GameObject;
				GameObject go = UnityEngine.Object.Instantiate(bundleGameObject);
				go.layer = LayerMask.NameToLayer(LayerNames.UI);
				UI ui = ComponentFactory.Create<UI, string, GameObject>(UIType.UIHUDText, go ,false);
				var com = ui.AddComponent<UIHUDTextComponent>();
				adapter = ui.AddComponent<UIHUDTextAdapterComponent>();
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
