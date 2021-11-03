using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient.requests
{
	[StructLayout(LayoutKind.Explicit, Size = 12)]
	unsafe struct ImpulsesAnswerHeader
	{
		public const int Size = 12;

		[FieldOffset(0)]
		public RequestHeader Header;

		[FieldOffset(4)]
		public int StationID;

		[FieldOffset(8)]
		public short TSCount;

		[FieldOffset(10)]
		public short TUCount;
	}
}
