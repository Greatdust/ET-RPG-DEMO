namespace ETModel
{
	[Config((int)(AppType.ClientM))]
	public partial class ExpForLevelUpCategory : ACategory<ExpForLevelUp>
	{
	}

	public class ExpForLevelUp: IConfig
	{
		public long Id { get; set; }
		public string Name;
		public int Exp;
	}
}
