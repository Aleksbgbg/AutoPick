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

        private WriteableBitmap _screenshotRenderSurface;

        private ChampionStore _championStore;

        private LaneImageFetcher _laneImageFetcher;

        private MainViewModel _mainViewModel;

        private DiskDataStore _dataStore;

        protected override void OnStartup(StartupEventArgs e)
        {
            DpiAwareness.Enable();

            base.OnStartup(e);
        }

        private async void AppStartup(object sender, StartupEventArgs e)
        {
            _singleInstance.TryConsume();

            if (_singleInstance.InstanceAlreadyLive())
            {
                IntPtr otherAppWindow = Win32Util.FindWindowA(null, "AutoPick");
                Win32Util.SetForegroundWindow(otherAppWindow);
                Shutdown();
                return;
            }

            await ChampionStore.LoadChampionsIfNecessary();

            _screenshotRenderSurface = ImageFactory.CreateScreenshotRenderSurface();
            _championStore = new ChampionStore();
            _laneImageFetcher = new LaneImageFetcher();
            _mainViewModel = new MainViewModel(_screenshotRenderSurface, _championStore.Champions);
            _dataStore = new DiskDataStore(_mainViewModel, _championStore);

            _dataStore.Load();

            MainWindow mainWindow = new(_championStore, _laneImageFetcher)
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
            new RemoteAppController(
                    this, mainWindow, _mainViewModel, new DetectionUpdateWaiter(autoPicker), _championStore)
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