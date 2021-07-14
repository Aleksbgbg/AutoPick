namespace AutoPick.WinApi.Native
{
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Rect
    {
        private readonly int _left;
        private readonly int _top;
        private readonly int _right;
        private readonly int _bottom;

        public int Width => _right - _left;
        public int Height => _bottom - _top;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Point
    {
        public Win32Point(int x, int y)
        {
            X = x;
            Y = y;
        }

        public int X { get; }
        public int Y { get; }
    }

    public enum SystemMetricParam
    {
        SM_CXSCREEN = 0,
        SM_CYSCREEN = 1
    }

    [Flags]
    public enum SetWindowPosFlags : uint
    {
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOREDRAW = 0x0008,
        SWP_NOACTIVATE = 0x0010,
        SWP_DRAWFRAME = 0x0020,
        SWP_FRAMECHANGED = 0x0020,
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_NOCOPYBITS = 0x0100,
        SWP_NOOWNERZORDER = 0x0200,
        SWP_NOREPOSITION = 0x0200,
        SWP_NOSENDCHANGING = 0x0400,
        SWP_DEFERERASE = 0x2000,
        SWP_ASYNCWINDOWPOS = 0x4000
    }

    public static class HwndInsertAfter
    {
        public static IntPtr HWND_TOPMOST = new(-1);

        public static IntPtr HWND_NOTOPMOST = new(-2);

        public static IntPtr HWND_TOP = IntPtr.Zero;

        public static IntPtr HWND_BOTTOM = new(1);
    }

    public enum GetWindowLongParam
    {
        GWL_EXSTYLE = -20,
        GWL_HINSTANCE = -6,
        GWL_HWNDPARENT = -8,
        GWL_ID = -12,
        GWL_STYLE = -16,
        GWL_USERDATA = -21,
        GWL_WNDPROC = -4
    }

    public enum DeviceScaleFactor
    {
        DEVICE_SCALE_FACTOR_INVALID,
        SCALE_100_PERCENT,
        SCALE_120_PERCENT,
        SCALE_125_PERCENT,
        SCALE_140_PERCENT,
        SCALE_150_PERCENT,
        SCALE_160_PERCENT,
        SCALE_175_PERCENT,
        SCALE_180_PERCENT,
        SCALE_200_PERCENT,
        SCALE_225_PERCENT,
        SCALE_250_PERCENT,
        SCALE_300_PERCENT,
        SCALE_350_PERCENT,
        SCALE_400_PERCENT,
        SCALE_450_PERCENT,
        SCALE_500_PERCENT
    }

    public enum MonitorFromWindowParam
    {
        MONITOR_DEFAULTTONULL = 0,
        MONITOR_DEFAULTTOPRIMARY = 1,
        MONITOR_DEFAULTTONEAREST = 2
    }

    public static partial class Win32Util
    {
        [DllImport(User32Dll)]
        public static extern IntPtr FindWindowA(string? windowClassName, string windowName);

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool IsIconic(IntPtr windowHandle);

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetWindowRect(IntPtr windowHandle, out Win32Rect rect);

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(IntPtr windowHandle, ref Win32Point point);

        [DllImport(User32Dll)]
        public static extern int GetSystemMetrics(SystemMetricParam systemMetric);

        public static int CalculateAbsoluteCoordinateX(int x)
        {
            return (x * 65536) / GetSystemMetrics(SystemMetricParam.SM_CXSCREEN);
        }

        public static int CalculateAbsoluteCoordinateY(int y)
        {
            return (y * 65536) / GetSystemMetrics(SystemMetricParam.SM_CYSCREEN);
        }

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetForegroundWindow(IntPtr windowHandle);

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SetWindowPos(
            IntPtr windowHandle,
            IntPtr hWndInsertAfter,
            int x,
            int y,
            int cx,
            int cy,
            SetWindowPosFlags flags);

        [DllImport(User32Dll)]
        public static extern IntPtr SendMessage(IntPtr windowHandle, Message message, IntPtr wParam, IntPtr lParam);

        [DllImport(User32Dll)]
        public static extern WindowStylesEx GetWindowLongA(IntPtr windowHandle, GetWindowLongParam param);

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LogicalToPhysicalPointForPerMonitorDPI(IntPtr windowHandle, ref Win32Point point);

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool LogicalToPhysicalPoint(IntPtr windowHandle, ref Win32Point point);

        [DllImport(User32Dll)]
        public static extern IntPtr MonitorFromWindow(
            IntPtr windowHandle,
            MonitorFromWindowParam flags
        );

        [DllImport("Shcore.dll")]
        public static extern IntPtr GetScaleFactorForMonitor(
            IntPtr            hMon,
            out DeviceScaleFactor scale
        );
    }
}