using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient.requests
{
	[StructLayout(LayoutKind.Explicit, Size = 44)]
	unsafe struct BroadcastCommandAnswer
	{
		public const int Size = 44;
		public const int CommandIDLength = 32;
		
		[FieldOffset(0)]
		public int PacketType;

		[FieldOffset(4)]
		public short PacketSize;

		[FieldOffset(6)]
		public int StationID;

		[FieldOffset(10)]
		public fixed byte CommandID[32];

		[FieldOffset(42)]
		public short RequestResult;
	}
}
