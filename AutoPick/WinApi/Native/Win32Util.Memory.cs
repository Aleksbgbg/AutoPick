namespace AutoPick.WinApi.Native
{
    using System;
    using System.Runtime.InteropServices;

    public partial class Win32Util
    {
        [DllImport(Kernel32Dll)]
        public static extern void RtlZeroMemory(IntPtr dest, int size);
    }
}