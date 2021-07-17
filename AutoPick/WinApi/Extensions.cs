namespace AutoPick.WinApi
{
    using System.Drawing;
    using AutoPick.WinApi.Native;

    public static class Extensions
    {
        public static Size ToSize(this Win32Rect win32Rect)
        {
            return new(win32Rect.Width, win32Rect.Height);
        }
    }
}