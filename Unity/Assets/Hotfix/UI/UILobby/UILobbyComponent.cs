using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
// Code Generate By Tool : 2019/4/3 15:34:30

namespace ETHotfix
{
	[ObjectSystem]
	public class UILobbyComponentSystem : AwakeSystem<UILobbyComponent>
	{
		public override void Awake(UILobbyComponent self)
		{
			self.Awake();
		}
	}

	public class UILobbyComponent : Component
	{
		#region 字段声明
		public Image Transfer1_Image;
		public Button Transfer1_Button;
		public Image Transfer2_Image;
		public Button Transfer2_Button;
		public Image Send_Image;
		public Button Send_Button;
		public Image SendRpc_Image;
		public Button SendRpc_Button;
		public Image EnterMap_Image;
		public Button EnterMap_Button;
		public Text Text_Text;
		#endregion

		public void Awake()
		{
			ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			Transfer1_Image = rc.Get<GameObject> ("Transfer1").GetComponent<Image> ();
			Transfer1_Button = rc.Get<GameObject> ("Transfer1").GetComponent<Button> ();
			Transfer2_Image = rc.Get<GameObject> ("Transfer2").GetComponent<Image> ();
			Transfer2_Button = rc.Get<GameObject> ("Transfer2").GetComponent<Button> ();
			Send_Image = rc.Get<GameObject> ("Send").GetComponent<Image> ();
			Send_Button = rc.Get<GameObject> ("Send").GetComponent<Button> ();
			SendRpc_Image = rc.Get<GameObject> ("SendRpc").GetComponent<Image> ();
			SendRpc_Button = rc.Get<GameObject> ("SendRpc").GetComponent<Button> ();
			EnterMap_Image = rc.Get<GameObject> ("EnterMap").GetComponent<Image> ();
			EnterMap_Button = rc.Get<GameObject> ("EnterMap").GetComponent<Button> ();
			Text_Text = rc.Get<GameObject> ("Text").GetComponent<Text> ();
	}

		public void OnTransfer1ButtonClick(Action action)
		{
			Transfer1_Button.onClick.Add(action);
		}

		public void OnTransfer2ButtonClick(Action action)
		{
			Transfer2_Button.onClick.Add(action);
		}

		public void OnSendButtonClick(Action action)
		{
			Send_Button.onClick.Add(action);
		}

		public void OnSendRpcButtonClick(Action action)
		{
			SendRpc_Button.onClick.Add(action);
		}

		public void OnEnterMapButtonClick(Action action)
		{
			EnterMap_Button.onClick.Add(action);
		}
	}
}
