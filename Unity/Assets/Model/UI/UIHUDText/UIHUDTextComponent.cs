using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
// Code Generate By Tool : 2019/4/16 22:14:30

namespace ETModel
{
	[ObjectSystem]
	public class UIHUDTextComponentSystem : AwakeSystem<UIHUDTextComponent>
	{
		public override void Awake(UIHUDTextComponent self)
		{
			self.Awake();
		}
	}

	public class UIHUDTextComponent : Component
	{
		#region 字段声明
		public Text Text_Text;
		public Selectable TextP_Selectable;
		#endregion

		public void Awake()
		{
			ReferenceCollector rc = GetParent<UI>().GameObject.GetComponent<ReferenceCollector>();
			Text_Text = rc.Get<GameObject> ("Text").GetComponent<Text> ();
			TextP_Selectable = rc.Get<GameObject> ("TextP").GetComponent<Selectable> ();
	}
	}
}
