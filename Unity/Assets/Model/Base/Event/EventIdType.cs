namespace ETModel
{
	public static class EventIdType
	{
		public const string RecvHotfixMessage = "RecvHotfixMessage";
		public const string BehaviorTreeRunTreeEvent = "BehaviorTreeRunTreeEvent";
		public const string BehaviorTreeOpenEditor = "BehaviorTreeOpenEditor";
		public const string BehaviorTreeClickNode = "BehaviorTreeClickNode";
		public const string BehaviorTreeAfterChangeNodeType = "BehaviorTreeAfterChangeNodeType";
		public const string BehaviorTreeCreateNode = "BehaviorTreeCreateNode";
		public const string BehaviorTreePropertyDesignerNewCreateClick = "BehaviorTreePropertyDesignerNewCreateClick";
		public const string BehaviorTreeMouseInNode = "BehaviorTreeMouseInNode";
		public const string BehaviorTreeConnectState = "BehaviorTreeConnectState";
		public const string BehaviorTreeReplaceClick = "BehaviorTreeReplaceClick";
		public const string BehaviorTreeRightDesignerDrag = "BehaviorTreeRightDesignerDrag";
		public const string SessionRecvMessage = "SessionRecvMessage";
		public const string NumbericChange = "NumbericChange";
		public const string MessageDeserializeFinish = "MessageDeserializeFinish";
		public const string SceneChange = "SceneChange";
		public const string FrameUpdate = "FrameUpdate";
		public const string LoadingBegin = "LoadingBegin";
		public const string LoadingFinish = "LoadingFinish";
		public const string TestHotfixSubscribMonoEvent = "TestHotfixSubscribMonoEvent";
		public const string MaxModelEvent = "MaxModelEvent";

        //以下被region包裹的,个人推荐分文件的做法,但是这里是DEMO.那就无所谓了~
        #region 数值相关
        
        public const string HPChanged = "HPChanged";
        public const string MPChanged = "MPChanged";

        public const string UnitLvUp = "UnitLvUp";

        public const string NumericUpdated = "NumericUpdated";

        #endregion

        #region 战斗相关
        public const string DisplayEnemy = "DisplayEnemy";
        public const string CalDamage = "CalDamage";
        public const string AttackMissing = "AttackMissing";
        public const string GiveDamage = "GiveDamage";
        public const string GiveHealth = "GiveHealth";
        public const string GiveMp = "GiveMp";

        public const string OnUnitDie = "OnUnitDie";

        public const string CharacterStateUpdate = "CharacterStateUpdate";
        public const string CancelPreAction = "CancelPreAction";
        #endregion

        #region 奖励相关
        public const string GiveItem = "GiveItem";
        public const string GiveEquip = "GiveEquip";
        public const string GiveExp = "GiveExp";
        #endregion

        #region 游戏流程相关
        public const string LoadAssets = "LoadAssets";
        #endregion

        #region 玩家相关
        public const string EquipUpdated = "EquipUpdated";
        public const string EquipInventoryIsFull = "EquipInventoryIsFull";
        public const string EquipInventoryUpdated = "EquipInventoryUpdated";

        public const string ItemInventoryUpdated = "ItemInventoryUpdated";
        public const string ItemInventoryIsFull = "ItemInventoryIsFull";
        #endregion

        #region 场景交互相关
        public const string ClickSceneObject = "ClickSceneObject";
        #endregion
    }
}