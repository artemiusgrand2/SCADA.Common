using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient
{
	[StructLayout(LayoutKind.Explicit, Size = 76)]
	unsafe struct BroadcastCommandRequest
	{
		public const int Size = 76;
		public const int CommandIDLength = 32;
		public const int SenderIDLength = 32;
		
		[FieldOffset(0)]
		public int PacketType;

		[FieldOffset(4)]
		public short PacketSize;

		[FieldOffset(6)]
		public fixed byte SenderID[32];

		[FieldOffset(38)]
		public int StationID;

		[FieldOffset(42)]
		public fixed byte CommandID[32];

		[FieldOffset(74)]
		public short CommandValue;
	}
}
