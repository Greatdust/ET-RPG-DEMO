using ETModel;
using UnityEngine;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;
// Code Generate By Tool : 2019/3/26 20:36:55

namespace ETModel
{
    [Event(EventIdType.GiveDamage)]
    public class UIHUDText_GiveDamageEvent : AEvent<long, GameCalNumericTool.DamageData[]>
    {
        public override async void Run(long a, GameCalNumericTool.DamageData[] b)
        {
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(a);
            var ui = await Game.Scene.GetComponent<UIComponent>().Create(UIType.UIHUDText, false);
            foreach (var v in b)
                ui.GetComponent<UIHUDTextAdapterComponent>().NextHUD(string.Format("-{0}", v.damageValue), Color.red, unit.GameObject.transform);

        }
    }

    [Event(EventIdType.AttackMissing)]
    public class UIHUDText_AttackMissingEvent : AEvent<long>
    {
        public override async void Run(long b)
        {
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(b);
            var ui = await Game.Scene.GetComponent<UIComponent>().Create(UIType.UIHUDText, false);
            ui.GetComponent<UIHUDTextAdapterComponent>().NextHUD(string.Format("MISS!"), Color.black, unit.GameObject.transform);

        }
    }

    [Event(EventIdType.GiveHealth)]
    public class UIHUDText_GiveHealthEvent : AEvent<long, int>
    {
        public override async void Run(long a, int b)
        {
            Unit unit = Game.Scene.GetComponent<UnitComponent>().Get(a);
            var ui = await Game.Scene.GetComponent<UIComponent>().Create(UIType.UIHUDText, false);
            ui.GetComponent<UIHUDTextAdapterComponent>().NextHUD(string.Format("+{0}",b), Color.green, unit.GameObject.transform);

        }
    }

    [ObjectSystem]
    public class UIHUDTextAdapterComponentUpdateSystem : FixedUpdateSystem<UIHUDTextAdapterComponent>
    {
        public override void FixedUpdate(UIHUDTextAdapterComponent self)
        {
            self.Update();
        }
    }

    public class UIHUDTextAdapterComponent : Component
    {

        public UIHUDTextComponent com;
        public class UIElement
        {
            public RectTransform textParentTrans;
            public Text text;
            public Transform followTrans;
        }
        public Queue<UIElement> textQueue;

        public List<UIElement> onUseQueue;

        private Transform parentRectTrans;
        private Vector3 hudOffset = new Vector3(0, 20, 0);

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

        public void Init(UIHUDTextComponent com)
        {
            this.com = com;
            parentRectTrans = com.TextP_Selectable.transform.parent.GetComponent<Transform>();
            textQueue = new Queue<UIElement>();
            onUseQueue = new List<UIElement>();
            for (int i = 0; i < 5; i++)
            {

                textQueue.Enqueue(CreateNewText());
            }
        }

        public UIElement CreateNewText()
        {
            UIElement uIElement = new UIElement();
            uIElement.textParentTrans = GameObject.Instantiate(com.TextP_Selectable.gameObject, parentRectTrans.transform).GetComponent<RectTransform>();
            uIElement.text = uIElement.textParentTrans.GetComponentInChildren<Text>();
            return uIElement;
        }

        public void NextHUD(string info, Color textColor, Transform trans)
        {
            UIElement textUI = null;
            if (textQueue.Count > 0)
            {
                textUI = textQueue.Dequeue();
            }
            else
            {
                textUI = CreateNewText();
            }

            textUI.followTrans = trans;
            textUI.text.text = info;
            textUI.text.color = textColor;
            CanvasGroup cg = textUI.text.GetComponent<CanvasGroup>();
            cg.alpha = 1;
            textUI.text.transform.localPosition = Vector3.zero;
            Vector3 aimPos = Camera.main.WorldToScreenPoint(textUI.followTrans.position) - new Vector3(Screen.width / 2, Screen.height / 2) + hudOffset;
            textUI.textParentTrans.localPosition = aimPos;
            textUI.textParentTrans.gameObject.SetActive(true);

            onUseQueue.Add(textUI);
            LeanTween.alphaCanvas(cg, 0, 2).setOnComplete(() =>
            {
                textUI.textParentTrans.gameObject.SetActive(false);
                textQueue.Enqueue(textUI);
                onUseQueue.Remove(textUI);
            });
            LeanTween.moveLocal(textUI.text.gameObject, new Vector3(0, 200, 0), 2);
        }


        public void Update()
        {
            if (Camera.main == null) return;
            if (onUseQueue.Count > 0)
            {
                for (int i = onUseQueue.Count - 1; i >= 0; i--)
                {
                    if (onUseQueue[i].followTrans == null) continue;
                    if (!onUseQueue[i].followTrans.gameObject.activeSelf)
                    {
                        continue;
                    }
                    var textUI = onUseQueue[i];
                    Vector3 aimPos = Camera.main.WorldToScreenPoint(textUI.followTrans.position) - new Vector3(Screen.width / 2, Screen.height / 2) + hudOffset;
                    //RectTransformUtility.ScreenPointToLocalPointInRectangle(parentRectTrans, aimPos, null, out var localPoint);
                    textUI.textParentTrans.localPosition = aimPos;
                }
            }

        }
    }
}
