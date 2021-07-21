namespace AutoPick
{
    using System;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using AutoPick.DebugTools;
    using AutoPick.Execution;
    using AutoPick.StateDetection.Imaging;
    using AutoPick.ViewModels;
    using AutoPick.Views;
    using AutoPick.WinApi;
    using AutoPick.WinApi.Native;

    public partial class App
    {
        private readonly SingleInstance _singleInstance = new();

        private readonly WriteableBitmap _screenshotRenderSurface;

        private readonly LaneImageFetcher _laneImageFetcher = new();

        private readonly MainViewModel _mainViewModel;

        private readonly DiskDataStore _dataStore;

        public App()
        {
            _screenshotRenderSurface = ImageFactory.CreateScreenshotRenderSurface();
            _mainViewModel = new MainViewModel(_screenshotRenderSurface);
            _dataStore = new DiskDataStore(_mainViewModel);
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            DpiAwareness.Enable();

            base.OnStartup(e);
        }

        private void AppStartup(object sender, StartupEventArgs e)
        {
            _singleInstance.TryConsume();

            if (_singleInstance.InstanceAlreadyLive())
            {
                IntPtr otherAppWindow = Win32Util.FindWindowA(null, "AutoPick");
                Win32Util.SetForegroundWindow(otherAppWindow);
                Shutdown();
                return;
            }

            _dataStore.Load();

            MainWindow mainWindow = new(_laneImageFetcher)
            {
                DataContext = _mainViewModel
            };
            mainWindow.Show();

            ScreenshotPreviewRenderer screenshotPreviewRenderer = new(_screenshotRenderSurface);
            AutoPicker autoPicker = AutoPicker.Run(_mainViewModel, _mainViewModel, screenshotPreviewRenderer);

            HotKey.Factory hotKeyFactory = HotKey.Factory.For(mainWindow);

            const HotkeyModifiers modifiers =
                HotkeyModifiers.MOD_SHIFT | HotkeyModifiers.MOD_ALT | HotkeyModifiers.MOD_CONTROL;

            HotKey deactivate = hotKeyFactory.Create(modifiers, VirtualKeyCode.VK_F2);
            HotKey reactivate = hotKeyFactory.Create(modifiers, VirtualKeyCode.VK_F3);

            deactivate.Activated += (_, _) => autoPicker.Disable();
            reactivate.Activated += (_, _) => autoPicker.Enable();

        #if DEBUG
            ErrorReporting.Init();
            new RemoteAppController(this, mainWindow, _mainViewModel, new DetectionUpdateWaiter(autoPicker))
                .BeginRemoteControl();
        #endif
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _dataStore.Save();
            _singleInstance.Release();
        }
    }
}