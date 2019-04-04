using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
// Code Generate By Tool : 2019/4/3 15:34:30

namespace ETHotfix
{

	public class UILobbyAdapterComponent : Component
	{

		public UILobbyComponent com;

		public void Init(UILobbyComponent com)
		{

			this.com = com ;
            com.OnEnterMapButtonClick(() =>
            {
                MapHelper.EnterMapAsync().Coroutine();
            });
		}

		public void Start()
		{

		}

		public ETTask OnEnable()
		{
			return ETTask.CompletedTask;
		}

		public ETTask OnDisable()
		{
			return ETTask.CompletedTask;
		}
	}
}
