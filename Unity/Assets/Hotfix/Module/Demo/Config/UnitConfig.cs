using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.ClientH |  AppType.ClientM | AppType.Gate | AppType.Map))]
	public partial class UnitConfigCategory : ACategory<UnitConfig>
	{
	}

	public class UnitConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public string Desc;
		public string ABPacketName;
		public string AssetName;
		public int Level;
		public int HPMax;
		public int MPMax;
		public int ArmorResist;
		public int MagicResist;
		public double HitRate;
		public double DodgeRate;
		public double MoveSpeed;
		public double CritRate;
		public double CritDamagePct;
		public double HP_LeechRate;
		public double MP_LeechRate;
		public int HP_Restore;
		public int MP_Restore;
		public int ATK;
		public int MagicATK;
		public string[] Skills;
	}
}
