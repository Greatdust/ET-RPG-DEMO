using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
// Code Generate By Tool : 2019/4/3 15:25:34

namespace ETHotfix
{
	[ObjectSystem]
	public class UILoginComponentSystem : AwakeSystem<UILoginComponent>
	{
		public override void Awake(UILoginComponent self)
		{
			self.Awake();
		}
	}

	public class UILoginComponent : Component
	{
		#region 字段声明
		public Image Account_Image;
		public InputField Account_InputField;
		public Image Password_Image;
		public InputField Password_InputField;
		public Image LoginBtn_Image;
		public Button LoginBtn_Button;
		#endregion

		public void Awake()
		{
			ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			Account_Image = rc.Get<GameObject> ("Account").GetComponent<Image> ();
			Account_InputField = rc.Get<GameObject> ("Account").GetComponent<InputField> ();
			Password_Image = rc.Get<GameObject> ("Password").GetComponent<Image> ();
			Password_InputField = rc.Get<GameObject> ("Password").GetComponent<InputField> ();
			LoginBtn_Image = rc.Get<GameObject> ("LoginBtn").GetComponent<Image> ();
			LoginBtn_Button = rc.Get<GameObject> ("LoginBtn").GetComponent<Button> ();
	}

		public void OnLoginBtnButtonClick(Action action)
		{
			LoginBtn_Button.onClick.Add(action);
		}
	}
}
