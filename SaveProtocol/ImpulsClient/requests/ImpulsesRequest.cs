using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient.requests
{
	[StructLayout(LayoutKind.Explicit, Size=8)]
	unsafe struct ImpulsesRequest
	{
		public const int Size = 8;

		[FieldOffset(0)]
		public RequestHeader Header;

		[FieldOffset(4)]
		public int StationID;
	}
}
