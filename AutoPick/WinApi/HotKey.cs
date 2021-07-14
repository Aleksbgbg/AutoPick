namespace AutoPick.WinApi
{
    using System;
    using System.Windows;
    using System.Windows.Interop;
    using AutoPick.DebugTools;
    using AutoPick.WinApi.Native;

    public class HotKey
    {
        private readonly int _id;

        private HotKey(int id, HwndSource windowHandleSource)
        {
            _id = id;
            windowHandleSource.AddHook(WindowsEventHandler);
        }

        public event EventHandler? Activated;

        private IntPtr WindowsEventHandler(IntPtr window, int message, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if ((message == (int)Message.WM_HOTKEY) && (wParam.ToInt32() == _id))
            {
                Activated?.Invoke(this, EventArgs.Empty);
                handled = true;
            }

            return IntPtr.Zero;
        }

        public class Factory
        {
            private readonly IntPtr _windowHandle;

            private readonly HwndSource _windowHandleSource;

            private int _currentId;

            private Factory(Window window)
            {
                _windowHandle = new WindowInteropHelper(window).Handle;
                _windowHandleSource = HwndSource.FromHwnd(_windowHandle);

                if (_windowHandleSource == null)
                {
                    ErrorReporting.ReportError("HwndSource null");
                    throw new InvalidOperationException("HwndSource is null");
                }
            }

            public static Factory For(Window window)
            {
                return new(window);
            }

            public HotKey Create(HotkeyModifiers modifiers, VirtualKeyCode virtualKeyCode)
            {
                ++_currentId;
                Win32Util.RegisterHotKey(_windowHandle, _currentId, modifiers, virtualKeyCode);
                return new HotKey(_currentId, _windowHandleSource);
            }
        }
    }
}