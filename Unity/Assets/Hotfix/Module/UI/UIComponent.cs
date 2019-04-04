using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using UnityEngine;

namespace ETHotfix
{
	[ObjectSystem]
	public class UIComponentAwakeSystem : AwakeSystem<UIComponent>
	{
		public override void Awake(UIComponent self)
		{
			self.Camera = Component.Global.transform.Find("UICamera").gameObject;
            self.Load();
		}
	}
	
	/// <summary>
	/// 管理所有UI
	/// </summary>
	public class UIComponent: Component
	{
		public GameObject Camera;

        private readonly Dictionary<string, IUIFactory> UiTypes = new Dictionary<string, IUIFactory>();
        private readonly Dictionary<string, UI> uis = new Dictionary<string, UI>();
        private readonly Stack<Dictionary<string, bool>> uiStatesStack = new Stack<Dictionary<string, bool>>();

        public void Load()
        {
            this.UiTypes.Clear();

            List<Type> types = Game.EventSystem.GetTypes();

            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(UIFactoryAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                UIFactoryAttribute attribute = attrs[0] as UIFactoryAttribute;
                if (UiTypes.ContainsKey(attribute.Type))
                {
                    Log.Debug($"已经存在同类UI Factory: {attribute.Type}");
                    throw new Exception($"已经存在同类UI Factory: {attribute.Type}");
                }
                object o = Activator.CreateInstance(type);
                IUIFactory factory = o as IUIFactory;
                if (factory == null)
                {
                    Log.Error($"{o.GetType().FullName} 没有继承 IUIFactory");
                    continue;
                }
                this.UiTypes.Add(attribute.Type, factory);
            }
        }

        public ETTask<UI> Create(string type, bool HideAll)
        {
            if (HideAll)
            {
                if (uis.Count > 0)
                {
                    foreach (var v in uis)
                    {
                        v.Value.GameObject.SetActive(false);
                    }
                }
            }
            return Create(type);
        }


        public async ETTask<UI> Create(string type)
        {
            try
            {
                UI ui;
                if (uis.TryGetValue(type, out ui))
                {
                    if (!ui.GameObject.activeSelf)
                        await UiTypes[type].OnEnable();
                    ui.GameObject.SetActive(true);
                    SetHighestOrder(type);
                    return ui;
                }
                ui = UiTypes[type].Create();
                uis.Add(type, ui);
                UiTypes[type].Start();//Start
                await UiTypes[type].OnEnable();//OnEnable
                ui.GameObject.SetActive(true);
                SetHighestOrder(type);
                return ui;
            }
            catch (Exception e)
            {
                throw new Exception($"{type} UI 错误: {e}");
            }
        }

        public async void Hide(string type)
        {
            UI ui;
            if (!uis.TryGetValue(type, out ui))
            {
                return;
            }
            await UiTypes[type].OnDisable();
            ui.GameObject.SetActive(false);
        }

        public void SetHighestOrder(string type)
        {
            UI ui;
            if (!uis.TryGetValue(type, out ui))
            {
                return;
            }
            ui.GameObject.transform.SetAsLastSibling();
        }

        public void HideAllUI()
        {
            Dictionary<string, bool> newStackElement = new Dictionary<string, bool>();
            foreach (string type in this.uis.Keys.ToArray())
            {
                UI ui;
                if (!this.uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                newStackElement.Add(type, ui.GameObject.activeSelf);
                ui.GameObject.SetActive(false);
            }
            uiStatesStack.Push(newStackElement);
        }
        public void DisplayAllUI()
        {
            Dictionary<string, bool> newStackElement = uiStatesStack.Pop();
            foreach (string type in this.uis.Keys.ToArray())
            {
                UI ui;
                if (!this.uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                if (!newStackElement.ContainsKey(type)) continue;
                ui.GameObject.SetActive(newStackElement[type]);
            }
        }


        public void Add(UI ui)
		{
			ui.GameObject.GetComponent<Canvas>().worldCamera = this.Camera.GetComponent<Camera>();
			
			this.uis.Add(ui.Name, ui);
			ui.Parent = this;
		}

		public void Remove(string name)
		{
			if (!this.uis.TryGetValue(name, out UI ui))
			{
				return;
			}
			this.uis.Remove(name);
			ui.Dispose();
		}
		
		public UI Get(string name)
		{
			UI ui = null;
			this.uis.TryGetValue(name, out ui);
			return ui;
		}

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            base.Dispose();
            foreach (string type in uis.Keys.ToArray())
            {
                UI ui;
                if (!uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                uis.Remove(type);
                ui.Dispose();
            }
            this.uis.Clear();
            this.UiTypes.Clear();
            this.uiStatesStack.Clear();
        }
    }
}