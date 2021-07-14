namespace AutoPick.WinApi.Native
{
    using System;
    using System.Runtime.InteropServices;

    [Flags]
    public enum PrintWindowParam : uint
    {
        PW_ENTIREWINDOW = 0x0000,
        PW_CLIENTONLY = 0x0001
    }

    public static partial class Win32Util
    {
        [DllImport(User32Dll, SetLastError = SetLastError)]
        public static extern IntPtr GetDC(IntPtr windowHandle);

        [DllImport(User32Dll, SetLastError = SetLastError)]
        public static extern int ReleaseDC(IntPtr windowHandle, IntPtr deviceContext);

        [DllImport(Gdi32Dll, SetLastError = SetLastError)]
        public static extern IntPtr CreateCompatibleDC(IntPtr deviceContext);

        [DllImport(Gdi32Dll, SetLastError = SetLastError)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteDC(IntPtr deviceContext);

        [DllImport(Gdi32Dll, SetLastError = SetLastError)]
        public static extern IntPtr CreateCompatibleBitmap(IntPtr deviceContext, int x, int y);

        [DllImport(Gdi32Dll, SetLastError = SetLastError)]
        public static extern IntPtr SelectObject(IntPtr deviceContext, IntPtr bitmap);

        [DllImport(Gdi32Dll, SetLastError = SetLastError)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool DeleteObject(IntPtr obj);

        [DllImport(User32Dll, SetLastError = SetLastError)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool PrintWindow(IntPtr windowHandle, IntPtr deviceContext, PrintWindowParam param);
    }
}