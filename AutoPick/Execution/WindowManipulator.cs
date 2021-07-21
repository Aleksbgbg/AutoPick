namespace AutoPick.Execution
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using AutoPick.DebugTools;
    using AutoPick.StateDetection;
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;
    using AutoPick.WinApi;
    using AutoPick.WinApi.Native;

    public class WindowManipulator : IDisposable, ILeagueClientManipulator
    {
        private const int ShortDelayMs = 100;
        private const int LongDelayMs = 200;
        private const int ExtraLongDelayMs = 1_000;

        private readonly ClickPoints _clickPoints;

        private readonly ScreenshotGenerator _screenshotGenerator;

        private readonly StateDetector _stateDetector;

        private readonly IntPtr _window;

        private readonly IntPtr _targetDeviceContext;
        private readonly IntPtr _bitmapHandle;

        private WindowManipulator(
            ClickPoints clickPoints,
            ScreenshotGenerator screenshotGenerator,
            StateDetector stateDetector,
            IntPtr window,
            IntPtr targetDeviceContext,
            IntPtr bitmapHandle)
        {
            _clickPoints = clickPoints;
            _screenshotGenerator = screenshotGenerator;
            _stateDetector = stateDetector;
            _window = window;
            _targetDeviceContext = targetDeviceContext;
            _bitmapHandle = bitmapHandle;
        }

        ~WindowManipulator()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            ReleaseUnmanagedResources();
            if (disposing)
            {
                _screenshotGenerator.Dispose();
            }
        }

        private void ReleaseUnmanagedResources()
        {
            Win32Util.DeleteObject(_bitmapHandle);
            Win32Util.DeleteDC(_targetDeviceContext);
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

        public static Size GetWindowSize(IntPtr window)
        {
            Win32Util.GetWindowRect(window, out Win32Rect windowRect);
            return windowRect.ToSize();
        }

        public static bool IsValidSize(IntPtr window)
        {
            Size windowSize = GetWindowSize(window);
            return (windowSize.Width >= 1024) && (windowSize.Height >= 576);
        }

        public static WindowManipulator Create(IntPtr window, Config config,
                                               ScreenshotPreviewRenderer screenshotPreviewRenderer)
        {
            Size windowSize = GetWindowSize(window);

            IntPtr sourceDeviceContext = Win32Util.GetDC(window);
            IntPtr targetDeviceContext = Win32Util.CreateCompatibleDC(sourceDeviceContext);

            IntPtr bitmapHandle = Win32Util.CreateCompatibleBitmap(sourceDeviceContext, windowSize.Width, windowSize.Height);
            Win32Util.SelectObject(targetDeviceContext, bitmapHandle);

            Win32Util.ReleaseDC(window, sourceDeviceContext);

            return new WindowManipulator(
                new ClickPoints(windowSize),
                new ScreenshotGenerator(windowSize, screenshotPreviewRenderer),
                new StateDetector(config),
                window,
                targetDeviceContext,
                bitmapHandle);
        }

        public State DetectWindowState()
        {
            return _stateDetector.Detect(TakeFreshWindowSnapshot());
        }

        public void UpdatePreview()
        {
            _screenshotGenerator.UpdateWindowPreview();
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

        public async Task CallLane(Lane lane)
        {
            InputQueue inputQueue = new(_window);

            inputQueue.ClickMouse(_clickPoints.ChatBox);
            await FlushAndDelay(inputQueue, ShortDelayMs);

            inputQueue.TypeText(lane.ToCallout());
            inputQueue.PressEnter();
            await FlushAndDelay(inputQueue, ShortDelayMs);
        }

        public async Task PickChampion(string championName)
        {
            InputQueue inputQueue = new(_window);

            inputQueue.ClickMouse(_clickPoints.SearchBox);
            await FlushAndDelay(inputQueue, LongDelayMs);

            inputQueue.TypeText(championName);
            await FlushAndDelay(inputQueue, ExtraLongDelayMs);

            inputQueue.ClickMouse(_clickPoints.FirstChampionSelectionImage);
            await FlushAndDelay(inputQueue, LongDelayMs);

            inputQueue.ClickMouse(_clickPoints.LockInButton);
            await FlushAndDelay(inputQueue, ShortDelayMs);
        }

        public async Task LockIn()
        {
            InputQueue inputQueue = new(_window);
            inputQueue.ClickMouse(_clickPoints.LockInButton);
            await FlushAndDelay(inputQueue, ShortDelayMs);
        }

        private static Task FlushAndDelay(InputQueue inputQueue, int delayMs)
        {
            inputQueue.Flush();
            return Task.Delay(delayMs);
        }

        private IImage TakeFreshWindowSnapshot()
        {
            Win32Util.PrintWindow(_window, _targetDeviceContext, PrintWindowParam.PW_CLIENTONLY);
            _screenshotGenerator.UpdateWindowSnapshot(Image.FromHbitmap(_bitmapHandle));
            return _screenshotGenerator.RetrieveSearchImage();
        }

        private static IntPtr FindLeagueWindow()
        {
            return Win32Util.FindWindowA(null, "League of Legends");
        }
    }
}