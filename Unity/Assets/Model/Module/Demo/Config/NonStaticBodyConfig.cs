namespace ETModel
{
	[Config((int)(AppType.Map))]
	public partial class NonStaticBodyConfigCategory : ACategory<NonStaticBodyConfig>
	{
	}

	public class NonStaticBodyConfig: IConfig
	{
		public long Id { get; set; }
		public string FilePath;
		public string Key;
	}
}
