using System;
using System.Runtime.InteropServices;

namespace ICMLuaUtilElevation.Native.Struct
{
    [StructLayout(LayoutKind.Explicit, Size = 0x40)]
    internal struct PEB_DATA
    {
        [FieldOffset(0x000)]
        public byte InheritedAddressSpace;
        [FieldOffset(0x001)]
        public byte ReadImageFileExecOptions;
        [FieldOffset(0x002)]
        public byte BeingDebugged;
        [FieldOffset(0x003)]
        public byte Spare;
        [FieldOffset(0x008)]
        public nint Mutant;
        [FieldOffset(0x010)]
        public nint ImageBaseAddress;
        [FieldOffset(0x018)]
        public nint Ldr;
        [FieldOffset(0x020)]
        public nint ProcessParameters;
        [FieldOffset(0x028)]
        public nint SubSystemData;
        [FieldOffset(0x030)]
        public nint ProcessHeap;
        [FieldOffset(0x038)]
        public nint FastPebLock;
    }
}
