using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient.requests
{
	/// <summary>
	/// Заголовок запроса на список таблиц
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size=6)]
	struct TablesListRequestHeader
	{
		public const int Size = 6;

		[FieldOffset(0)]
		public RequestHeader Header;

		[FieldOffset(4)]
		public short StationsCount;
	}
}
