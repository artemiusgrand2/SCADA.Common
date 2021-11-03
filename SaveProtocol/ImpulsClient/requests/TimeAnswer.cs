using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.InteropServices;

namespace SCADA.Common.ImpulsClient.requests
{
    [StructLayout(LayoutKind.Explicit, Size = 12)]
    unsafe struct TimeAnswer
    {
        public const int Size = 12;

        [FieldOffset(0)]
        public RequestHeader Header;

        [FieldOffset(4)]
        public long Time;

    }
}
