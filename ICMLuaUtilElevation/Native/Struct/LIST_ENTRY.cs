using System;
using System.Runtime.InteropServices;

namespace ICMLuaUtilElevation.Native.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LIST_ENTRY
    {
        public nint Flink;
        public nint Blink;
    }
}
