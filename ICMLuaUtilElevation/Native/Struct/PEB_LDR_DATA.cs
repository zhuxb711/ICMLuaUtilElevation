using System;
using System.Runtime.InteropServices;

namespace ICMLuaUtilElevation.Native.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct PEB_LDR_DATA
    {
        public uint Length;
        public byte Initialized;
        public nint SsHandle;
        public LIST_ENTRY InLoadOrderModuleList;
        public LIST_ENTRY InMemoryOrderModuleList;
        public LIST_ENTRY InInitializationOrderModuleList;
        public nint EntryInProgress;
    }
}
