namespace AutoPick.Execution
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using AutoPick.Debug;
    using AutoPick.StateDetection;
    using AutoPick.StateDetection.Definition;
    using AutoPick.Win32;
    using AutoPick.Win32.Native;
    using Emgu.CV;
    using Emgu.CV.Structure;

    public class WindowManipulator
    {
        private const int ShortDelayMs = 100;
        private const int LongDelayMs = 200;

        private readonly StateDetector _stateDetector;

        private readonly ClickPoints _clickPoints;

        private readonly IntPtr _window;

        private readonly IntPtr _sourceDeviceContext;
        private readonly IntPtr _targetDeviceContext;

        private readonly IntPtr _bitmap;

        private WindowManipulator(StateDetector stateDetector, ClickPoints clickPoints, IntPtr window, IntPtr sourceDeviceContext, IntPtr targetDeviceContext, IntPtr bitmap)
        {
            _stateDetector = stateDetector;
            _clickPoints = clickPoints;
            _window = window;
            _sourceDeviceContext = sourceDeviceContext;
            _targetDeviceContext = targetDeviceContext;
            _bitmap = bitmap;
        }

        public static bool HasWindow(out IntPtr window)
        {
            window = FindLeagueWindow();
            return window.ToInt32() != 0;
        }

        public static bool IsMinimised(IntPtr window)
        {
            return Win32Util.IsIconic(window);
        }

        public static Win32Rect GetWindowSize(IntPtr window)
        {
            Win32Util.GetWindowRect(window, out Win32Rect windowRect);
            return windowRect;
        }

        public static bool IsValidSize(IntPtr window)
        {
            Win32Util.GetWindowRect(window, out Win32Rect windowRect);

            bool widthValid = (windowRect.Width == 1280) || (windowRect.Width == 1920);
            bool heightValid = (windowRect.Height == 720) || (windowRect.Height == 1080);

            return widthValid && heightValid;
        }

        public static WindowManipulator Create(IntPtr window, Config config)
        {
            IntPtr sourceDeviceContext = Win32Util.GetDC(window);
            IntPtr targetDeviceContext = Win32Util.CreateCompatibleDC(sourceDeviceContext);

            Win32Util.GetWindowRect(window, out Win32Rect windowRect);

            IntPtr bitmap = Win32Util.CreateCompatibleBitmap(sourceDeviceContext, windowRect.Width, windowRect.Height);
            Win32Util.SelectObject(targetDeviceContext, bitmap);

            return new WindowManipulator(
                new StateDetector(config),
                new ClickPoints(new Size(windowRect.Width, windowRect.Height)),
                window, sourceDeviceContext, targetDeviceContext, bitmap);
        }

        public void Delete()
        {
            Win32Util.DeleteObject(_bitmap);
            Win32Util.DeleteDC(_targetDeviceContext);
            Win32Util.ReleaseDC(_window, _sourceDeviceContext);
        }

        public State DetectWindowState()
        {
            return _stateDetector.Detect(TakeSnapshot());
        }

        public void AttemptToBringLeagueToForeground()
        {
            bool success = Win32Util.SetWindowPos(_window, HwndInsertAfter.HWND_TOPMOST, 0, 0, 0, 0,
                                                  SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE |
                                                  SetWindowPosFlags.SWP_NOMOVE);
            if (!success)
            {
                ErrorReporting.ReportError("Could not bring league to foreground");
            }
        }

        public void RestoreLeague()
        {
            bool success = Win32Util.SetWindowPos(_window, HwndInsertAfter.HWND_NOTOPMOST, 0, 0, 0, 0,
                                                  SetWindowPosFlags.SWP_NOACTIVATE | SetWindowPosFlags.SWP_NOSIZE |
                                                  SetWindowPosFlags.SWP_NOMOVE);
            if (!success)
            {
                ErrorReporting.ReportError("Couldn't restore league");
            }
        }

        public Task AcceptMatch()
        {
            InputQueue inputQueue = new(_window);
            inputQueue.ClickMouse(_clickPoints.AcceptButton);
            inputQueue.Flush();

            return Task.CompletedTask;
        }

        public async Task CallLane(string lane)
        {
            InputQueue inputQueue = new(_window);

            inputQueue.ClickMouse(_clickPoints.ChatBox);
            await FlushAndDelay(inputQueue, ShortDelayMs);

            inputQueue.TypeText(lane);
            inputQueue.PressEnter();
            inputQueue.Flush();
        }

        public async Task PickChampion(string championName)
        {
            InputQueue inputQueue = new(_window);

            inputQueue.ClickMouse(_clickPoints.SearchBox);
            await FlushAndDelay(inputQueue, ShortDelayMs);

            inputQueue.TypeText(championName);
            await FlushAndDelay(inputQueue, ShortDelayMs);

            inputQueue.ClickMouse(_clickPoints.FirstChampionSelectionImage);
            await FlushAndDelay(inputQueue, LongDelayMs);

            inputQueue.ClickMouse(_clickPoints.LockInButton);
            inputQueue.Flush();
        }

        public Task LockIn()
        {
            InputQueue inputQueue = new(_window);
            inputQueue.ClickMouse(_clickPoints.LockInButton);
            inputQueue.Flush();

            return Task.CompletedTask;
        }

        private static Task FlushAndDelay(InputQueue inputQueue, int delayMs)
        {
            inputQueue.Flush();
            return Task.Delay(delayMs);
        }

        private Image<Gray, byte> TakeSnapshot()
        {
            Win32Util.PrintWindow(_window, _targetDeviceContext, PrintWindowParam.PW_CLIENTONLY);
            return Image.FromHbitmap(_bitmap).ToImage<Gray, byte>();
        }

        private static IntPtr FindLeagueWindow()
        {
            return Win32Util.FindWindowA(null, "League of Legends");
        }
    }
}