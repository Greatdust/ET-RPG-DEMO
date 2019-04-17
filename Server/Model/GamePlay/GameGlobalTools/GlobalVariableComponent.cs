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

        public void Awake()
		{
            instance = this;
            Load();
        }

        public void Load()
        {
            globalVars = new GameGlobalVariable();
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