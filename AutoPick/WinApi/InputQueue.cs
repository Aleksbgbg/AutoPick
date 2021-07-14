namespace AutoPick.WinApi
{
    using System;
    using System.Collections.Generic;
    using AutoPick.WinApi.Native;

    public class InputQueue
    {
        private readonly List<Win32Input> _inputs = new();

        private readonly IntPtr _window;

        public InputQueue(IntPtr window)
        {
            _window = window;
        }

        public void Flush()
        {
            Win32Util.SendInput((uint)_inputs.Count, _inputs.ToArray(), Win32Input.Size);
            _inputs.Clear();
        }

        public void ClickMouse(Win32Point clickPoint)
        {
            Win32Util.ClientToScreen(_window, ref clickPoint);

            Win32Input input = new();
            input.type = SendInputEventType.INPUT_MOUSE;
            input.union.mi.dx = Win32Util.CalculateAbsoluteCoordinateX(clickPoint.X);
            input.union.mi.dy = Win32Util.CalculateAbsoluteCoordinateY(clickPoint.Y);
            input.union.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_ABSOLUTE | MouseEventFlags.MOUSEEVENTF_MOVE |
                                     MouseEventFlags.MOUSEEVENTF_LEFTDOWN | MouseEventFlags.MOUSEEVENTF_LEFTUP;
            _inputs.Add(input);
        }

        public void MoveMouse(Win32Point clickPoint)
        {
            Win32Util.ClientToScreen(_window, ref clickPoint);

            Win32Input input = new();
            input.type = SendInputEventType.INPUT_MOUSE;
            input.union.mi.dx = Win32Util.CalculateAbsoluteCoordinateX(clickPoint.X);
            input.union.mi.dy = Win32Util.CalculateAbsoluteCoordinateY(clickPoint.Y);
            input.union.mi.dwFlags = MouseEventFlags.MOUSEEVENTF_ABSOLUTE | MouseEventFlags.MOUSEEVENTF_MOVE;
            _inputs.Add(input);
        }

        public void TypeText(string text)
        {
            foreach (char c in text)
            {
                Win32Input input = new();
                input.type = SendInputEventType.INPUT_KEYBOARD;
                input.union.ki.wScan = c;
                input.union.ki.dwFlags = KeyEventFlags.KEYEVENTF_UNICODE;
                _inputs.Add(input);

                input.union.ki.dwFlags |= KeyEventFlags.KEYEVENTF_KEYUP;
                _inputs.Add(input);
            }
        }

        public void PressEnter()
        {
            Win32Input input = new();
            input.type = SendInputEventType.INPUT_KEYBOARD;
            input.union.ki.wVk = VirtualKeyCode.VK_RETURN;
            _inputs.Add(input);

            input.union.ki.dwFlags = KeyEventFlags.KEYEVENTF_KEYUP;
            _inputs.Add(input);
        }
    }
}