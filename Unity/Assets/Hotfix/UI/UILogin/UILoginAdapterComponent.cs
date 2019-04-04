using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
// Code Generate By Tool : 2019/4/3 15:25:34

namespace ETHotfix
{

	public class UILoginAdapterComponent : Component
	{

		public UILoginComponent com;

		public void Init(UILoginComponent com)
		{

			this.com = com ;
            com.OnLoginBtnButtonClick(() =>
            {
                LoginHelper.OnLoginAsync(this.com.Account_InputField.text).Coroutine();
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
