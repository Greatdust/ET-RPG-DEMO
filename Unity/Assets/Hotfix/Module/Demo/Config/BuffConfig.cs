using ETModel;

namespace ETHotfix
{
	[Config((int)(AppType.ClientM))]
	public partial class BuffConfigCategory : ACategory<BuffConfig>
	{
	}

	public class BuffConfig: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public string Desc;
	}
}
