namespace AutoPick.WinApi.Native
{
    public static partial class Win32Util
    {
        private const string User32Dll = "User32.dll";
        private const string Gdi32Dll = "Gdi32.dll";
        private const string ShCoreDll = "SHCore.dll";
        private const string Kernel32Dll = "Kernel32.dll";

    #if DEBUG
        private const bool SetLastError = true;
    #else
        private const bool SetLastError = false;
    #endif
    }
}