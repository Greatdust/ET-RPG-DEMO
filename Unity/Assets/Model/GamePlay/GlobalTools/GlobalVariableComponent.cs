using System;
using System.Collections.Generic;
using UnityEngine;

namespace ETModel
{
	[ObjectSystem]
	public class GlobalVariableComponentAwakeSystem : AwakeSystem<GlobalVariableComponent>
	{
		public override void Awake(GlobalVariableComponent self)
		{
			self.Awake();
		}
	}

	public class GlobalVariableComponent : Component
	{
        private static GlobalVariableComponent instance;
        public static GlobalVariableComponent Instance
        {
            get
            {
                return instance;
            }
        }
        public GameGlobalVariable globalVars;

        public const string ABName = "globalvar.unity3d";

        public void Awake()
		{
            instance = this;
            Load();
        }

        public async void Load()
        {
            await Game.Scene.GetComponent<ResourcesComponent>().LoadBundleAsync(ABName);
            globalVars = Game.Scene.GetComponent<ResourcesComponent>().GetAsset(ABName, "GolbalVarCollection") as GameGlobalVariable;
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle(ABName);
        }

		public override void Dispose()
		{
            globalVars = null;
            instance = null;
            if (this.IsDisposed)
			{
				return;
			}
			base.Dispose();

        }
	}
}