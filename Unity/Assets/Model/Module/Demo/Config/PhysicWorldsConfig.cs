namespace ETModel
{
	[Config((int)(AppType.Map))]
	public partial class PhysicWorldsConfigCategory : ACategory<PhysicWorldsConfig>
	{
	}

	public class PhysicWorldsConfig: IConfig
	{
		public long Id { get; set; }
		public string StaticObjs_Box;
	}
}
