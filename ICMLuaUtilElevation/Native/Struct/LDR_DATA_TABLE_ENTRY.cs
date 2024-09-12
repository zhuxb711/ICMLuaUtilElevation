using System;
using System.Runtime.InteropServices;
using Vanara.PInvoke;

namespace ICMLuaUtilElevation.Native.Struct
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct LDR_DATA_TABLE_ENTRY
    {
        public LIST_ENTRY InLoadOrderLinks;
        public LIST_ENTRY InMemoryOrderLinks;
        public LIST_ENTRY InInitializationOrderLinks;
        public nint DllBase;
        public nint EntryPoint;
        public uint SizeOfImage;
        public NtDll.UNICODE_STRING FullDllName;
        public NtDll.UNICODE_STRING BaseDllName;
    }
}
