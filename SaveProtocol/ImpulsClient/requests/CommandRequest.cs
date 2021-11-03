using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient.requests
{
	[StructLayout(LayoutKind.Explicit, Size=106)]
	unsafe struct CommandRequest
	{
		public const int Size = 106;
		public const int CommandIDLength = 32;
		public const int SenderIDLength = 64;

		[FieldOffset(0)]
		public RequestHeader Header;

		[FieldOffset(4)]
		public fixed byte SenderID[64];

		[FieldOffset(68)]
		public int StationID;

		[FieldOffset(72)]
		public fixed byte CommandID[32];

		[FieldOffset(104)]
		public short CommandValue;
	}
}
