using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient.requests
{
	[StructLayout(LayoutKind.Explicit, Size=4)]
	public unsafe struct RequestHeader
	{
		public const int Size = 4;
		//public const int SenderIDLength = 32;

		[FieldOffset(0)]
		public short PacketType;
		
		[FieldOffset(2)]
		public short PacketSize;

		//[FieldOffset(8)]
		//public fixed byte SenderID[32];
	}
}
