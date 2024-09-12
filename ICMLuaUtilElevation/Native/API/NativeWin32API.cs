using System;
using System.Runtime.InteropServices;

namespace ICMLuaUtilElevation
{
    internal static partial class NativeWin32API
    {
        [LibraryImport("ntdll.dll")]
        public static partial void RtlEnterCriticalSection(IntPtr lpCriticalSection);

        [LibraryImport("ntdll.dll")]
        public static partial void RtlLeaveCriticalSection(IntPtr lpCriticalSection);
    }
}
