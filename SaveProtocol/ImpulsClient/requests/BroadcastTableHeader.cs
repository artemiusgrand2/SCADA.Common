using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient
{
	[StructLayout(LayoutKind.Explicit, Size = 14)]
	unsafe struct BroadcastTableHeader
	{
		public const int Size = 14;

		[FieldOffset(0)]
		public int PacketType;

		[FieldOffset(4)]
		public short PacketSize;
		
		[FieldOffset(6)]
		public int StationID;

		[FieldOffset(10)]
		public short StartIndex;

		[FieldOffset(12)]
		public short Count;
	}
}
