namespace AutoPick.WinApi
{
    using System;
    using AutoPick.WinApi.Native;

    public static class DpiAwareness
    {
        public static void Enable()
        {
            // Windows 8.1 added support for per monitor DPI
            if (Environment.OSVersion.Version >= new Version(6, 3, 0))
            {
                // Windows 10 creators update added support for per monitor v2
                if (Environment.OSVersion.Version >= new Version(10, 0, 15063))
                {
                    Win32Util.SetProcessDpiAwarenessContext((int)DPI_AWARENESS_CONTEXT.DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2);
                }
                else
                {
                    Win32Util.SetProcessDpiAwareness(PROCESS_DPI_AWARENESS.Process_Per_Monitor_DPI_Aware);
                }
            }
            else
            {
                Win32Util.SetProcessDPIAware();
            }
        }
    }
}