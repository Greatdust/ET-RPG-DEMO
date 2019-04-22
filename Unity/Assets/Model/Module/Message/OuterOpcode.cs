using ETModel;
namespace ETModel
{
	[Message(OuterOpcode.Actor_Test)]
	public partial class Actor_Test : IActorMessage {}

	[Message(OuterOpcode.C2M_TestRequest)]
	public partial class C2M_TestRequest : IActorLocationRequest {}

	[Message(OuterOpcode.M2C_TestResponse)]
	public partial class M2C_TestResponse : IActorLocationResponse {}

	[Message(OuterOpcode.Actor_TransferRequest)]
	public partial class Actor_TransferRequest : IActorLocationRequest {}

	[Message(OuterOpcode.Actor_TransferResponse)]
	public partial class Actor_TransferResponse : IActorLocationResponse {}

	[Message(OuterOpcode.C2G_EnterMap)]
	public partial class C2G_EnterMap : IRequest {}

	[Message(OuterOpcode.G2C_EnterMap)]
	public partial class G2C_EnterMap : IResponse {}

// 自己的unit id
// 所有的unit
	[Message(OuterOpcode.C2R_HeartBeat)]
	public partial class C2R_HeartBeat : IRequest {}

	[Message(OuterOpcode.R2C_HeartBeat)]
	public partial class R2C_HeartBeat : IResponse {}

	[Message(OuterOpcode.UnitInfo)]
	public partial class UnitInfo {}

	[Message(OuterOpcode.UnitNumeric)]
	public partial class UnitNumeric {}

	[Message(OuterOpcode.M2C_CreateUnits)]
	public partial class M2C_CreateUnits : IActorMessage {}

	[Message(OuterOpcode.Frame_ClickMap)]
	public partial class Frame_ClickMap : IActorLocationMessage {}

	[Message(OuterOpcode.InputResult_Move)]
	public partial class InputResult_Move : IActorMessage {}

	[Message(OuterOpcode.Input_Move)]
	public partial class Input_Move : IActorLocationMessage {}

	[Message(OuterOpcode.Input_UseSkill_Pos)]
	public partial class Input_UseSkill_Pos : IActorLocationMessage {}

	[Message(OuterOpcode.Input_UseSkill_Tar)]
	public partial class Input_UseSkill_Tar : IActorLocationMessage {}

	[Message(OuterOpcode.Input_UseSkill_Dir)]
	public partial class Input_UseSkill_Dir : IActorLocationMessage {}

	[Message(OuterOpcode.InputResult_UseSkill_Dir)]
	public partial class InputResult_UseSkill_Dir : IActorMessage {}

	[Message(OuterOpcode.M2C_Pushback)]
	public partial class M2C_Pushback : IActorMessage {}

	[Message(OuterOpcode.M2C_HitEffect)]
	public partial class M2C_HitEffect : IActorMessage {}

	[Message(OuterOpcode.M2C_PlayEffect)]
	public partial class M2C_PlayEffect : IActorMessage {}

	[Message(OuterOpcode.M2C_PlaySound)]
	public partial class M2C_PlaySound : IActorMessage {}

	[Message(OuterOpcode.M2C_GiveDamage)]
	public partial class M2C_GiveDamage : IActorMessage {}

	[Message(OuterOpcode.M2C_GiveAttackMissing)]
	public partial class M2C_GiveAttackMissing : IActorMessage {}

	[Message(OuterOpcode.DamageData)]
	public partial class DamageData {}

	[Message(OuterOpcode.M2C_GiveHealth)]
	public partial class M2C_GiveHealth : IActorMessage {}

	[Message(OuterOpcode.M2C_InterruptSkill)]
	public partial class M2C_InterruptSkill : IActorMessage {}

	[Message(OuterOpcode.M2C_DisposeEmitObj)]
	public partial class M2C_DisposeEmitObj : IActorMessage {}

	[Message(OuterOpcode.Vector3Info)]
	public partial class Vector3Info {}

	[Message(OuterOpcode.M2C_PathfindingResult)]
	public partial class M2C_PathfindingResult : IActorMessage {}

	[Message(OuterOpcode.C2R_Ping)]
	public partial class C2R_Ping : IRequest {}

	[Message(OuterOpcode.R2C_Ping)]
	public partial class R2C_Ping : IResponse {}

	[Message(OuterOpcode.G2C_Test)]
	public partial class G2C_Test : IMessage {}

	[Message(OuterOpcode.C2M_Reload)]
	public partial class C2M_Reload : IRequest {}

	[Message(OuterOpcode.M2C_Reload)]
	public partial class M2C_Reload : IResponse {}

}
namespace ETModel
{
	public static partial class OuterOpcode
	{
		 public const ushort Actor_Test = 101;
		 public const ushort C2M_TestRequest = 102;
		 public const ushort M2C_TestResponse = 103;
		 public const ushort Actor_TransferRequest = 104;
		 public const ushort Actor_TransferResponse = 105;
		 public const ushort C2G_EnterMap = 106;
		 public const ushort G2C_EnterMap = 107;
		 public const ushort C2R_HeartBeat = 108;
		 public const ushort R2C_HeartBeat = 109;
		 public const ushort UnitInfo = 110;
		 public const ushort UnitNumeric = 111;
		 public const ushort M2C_CreateUnits = 112;
		 public const ushort Frame_ClickMap = 113;
		 public const ushort InputResult_Move = 114;
		 public const ushort Input_Move = 115;
		 public const ushort Input_UseSkill_Pos = 116;
		 public const ushort Input_UseSkill_Tar = 117;
		 public const ushort Input_UseSkill_Dir = 118;
		 public const ushort InputResult_UseSkill_Dir = 119;
		 public const ushort M2C_Pushback = 120;
		 public const ushort M2C_HitEffect = 121;
		 public const ushort M2C_PlayEffect = 122;
		 public const ushort M2C_PlaySound = 123;
		 public const ushort M2C_GiveDamage = 124;
		 public const ushort M2C_GiveAttackMissing = 125;
		 public const ushort DamageData = 126;
		 public const ushort M2C_GiveHealth = 127;
		 public const ushort M2C_InterruptSkill = 128;
		 public const ushort M2C_DisposeEmitObj = 129;
		 public const ushort Vector3Info = 130;
		 public const ushort M2C_PathfindingResult = 131;
		 public const ushort C2R_Ping = 132;
		 public const ushort R2C_Ping = 133;
		 public const ushort G2C_Test = 134;
		 public const ushort C2M_Reload = 135;
		 public const ushort M2C_Reload = 136;
	}
}
