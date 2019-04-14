using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.Map))]
	public partial class DungeonConfigCategory : ACategory<DungeonConfig>
	{
	}

	public class DungeonConfig: IConfig
	{
		public long Id { get; set; }
		public string ConfigPath_Server;
		public string ConfigFileName;
	}
}
