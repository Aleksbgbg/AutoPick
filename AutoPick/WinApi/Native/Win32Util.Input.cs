namespace AutoPick.WinApi.Native
{
    using System;
    using System.Runtime.InteropServices;

    public enum SendInputEventType : uint
    {
        INPUT_MOUSE = 0,
        INPUT_KEYBOARD = 1,
        INPUT_HARDWARE = 2
    }

    [Flags]
    public enum KeyEventFlags : uint
    {
        KEYEVENTF_EXTENDEDKEY = 0x0001,
        KEYEVENTF_KEYUP = 0x0002,
        KEYEVENTF_SCANCODE = 0x0008,
        KEYEVENTF_UNICODE = 0x0004
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32Input
    {
        public static readonly int Size = Marshal.SizeOf(typeof(Win32Input));

        public SendInputEventType type;

        public InputUnion union;
    }

    [StructLayout(LayoutKind.Explicit)]
    public struct InputUnion
    {
        [FieldOffset(0)] public Win32MouseInput mi;

        [FieldOffset(0)] public Win32KeyboardInput ki;

        [FieldOffset(0)] public Win32HardwareInput hi;
    }

    [Flags]
    public enum MouseEventFlags : uint
    {
        MOUSEEVENTF_MOVE = 0x0001,
        MOUSEEVENTF_LEFTDOWN = 0x0002,
        MOUSEEVENTF_LEFTUP = 0x0004,
        MOUSEEVENTF_RIGHTDOWN = 0x0008,
        MOUSEEVENTF_RIGHTUP = 0x0010,
        MOUSEEVENTF_MIDDLEDOWN = 0x0020,
        MOUSEEVENTF_MIDDLEUP = 0x0040,
        MOUSEEVENTF_XDOWN = 0x0080,
        MOUSEEVENTF_XUP = 0x0100,
        MOUSEEVENTF_WHEEL = 0x0800,
        MOUSEEVENTF_VIRTUALDESK = 0x4000,
        MOUSEEVENTF_ABSOLUTE = 0x8000
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32MouseInput
    {
        public int dx;
        public int dy;
        public uint mouseData;
        public MouseEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32KeyboardInput
    {
        public VirtualKeyCode wVk;
        public ushort wScan;
        public KeyEventFlags dwFlags;
        public uint time;
        public IntPtr dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct Win32HardwareInput
    {
        public int uMsg;
        public short wParamL;
        public short wParamH;
    }

    [Flags]
    public enum HotkeyModifiers : uint
    {
        MOD_NONE = 0x0000,
        MOD_ALT = 0x0001,
        MOD_CONTROL = 0x0002,
        MOD_SHIFT = 0x0004,
        MOD_WIN = 0x0008,
        MOD_NOREPEAT = 0x4000
    }

    public static partial class Win32Util
    {
        [DllImport(User32Dll)]
        public static extern uint SendInput(uint inputsLength,
                                            [MarshalAs(UnmanagedType.LPArray)] [In]
                                            Win32Input[] inputs,
                                            int inputSize);

        [DllImport(User32Dll)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool RegisterHotKey(IntPtr windowHandle,
                                                 int hotKeyId,
                                                 HotkeyModifiers modifiers,
                                                 VirtualKeyCode virtualKeyCode);
    }
}