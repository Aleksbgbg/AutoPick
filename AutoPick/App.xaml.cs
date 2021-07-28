namespace AutoPick
{
    using System;
    using System.Linq;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using AutoPick.Converters;
    using AutoPick.DebugTools;
    using AutoPick.Execution;
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;
    using AutoPick.Util;
    using AutoPick.ViewModels;
    using AutoPick.Views;
    using AutoPick.WinApi;
    using AutoPick.WinApi.Native;

    public partial class App
    {
        private readonly SingleInstance _singleInstance = new();

        private DiskDataStore? _dataStore;

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

        #if DEBUG
            ErrorReporting.Init();
        #endif

            await ChampionStore.LoadChampionsIfNecessary();

            WriteableBitmap screenshotRenderSurface = ImageFactory.CreateScreenshotRenderSurface();
            ChampionStore championStore = new();
            LaneImageFetcher laneImageFetcher = new();
            MainViewModel mainViewModel = new(screenshotRenderSurface, championStore.Champions);
            _dataStore = new DiskDataStore(mainViewModel, championStore);

            _dataStore.Load();

            StateInfoDisplay[] infoDisplays = Enum.GetValues<State>().Zip(typeof(State).GetMembers(BindingFlags.Static | BindingFlags.Public))
                                                  .Select(zippedValue => new StateInfoDisplay(
                                                              zippedValue.First,
                                                              zippedValue.Second.GetCustomAttribute<InfoDisplayAttribute>()))
                                                  .ToArray();

            ChampionImageConverter championImageConverter = new(championStore);
            LaneImageConverter laneImageConverter = new(laneImageFetcher);
            InfoTextConverter infoTextConverter = new(infoDisplays);
            InfoIconConverter infoIconConverter = new(infoDisplays);

            View.Register(() => new CalloutsView(championImageConverter, laneImageConverter)
            {
                DataContext = mainViewModel
            });
            View.Register(() => new StateView(infoTextConverter, infoIconConverter)
            {
                DataContext = mainViewModel
            });

            MainWindow mainWindow = new()
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();

            ScreenshotPreviewRenderer screenshotPreviewRenderer = new(screenshotRenderSurface);
            AutoPicker autoPicker = AutoPicker.Run(mainViewModel, mainViewModel, screenshotPreviewRenderer);

            HotKey.Factory hotKeyFactory = HotKey.Factory.For(mainWindow);

            const HotkeyModifiers modifiers =
                HotkeyModifiers.MOD_SHIFT | HotkeyModifiers.MOD_ALT | HotkeyModifiers.MOD_CONTROL;

            HotKey deactivate = hotKeyFactory.Create(modifiers, VirtualKeyCode.VK_F2);
            HotKey reactivate = hotKeyFactory.Create(modifiers, VirtualKeyCode.VK_F3);

            deactivate.Activated += (_, _) => autoPicker.Disable();
            reactivate.Activated += (_, _) => autoPicker.Enable();

        #if DEBUG
            new RemoteAppController(
                    this, mainWindow, mainViewModel, new DetectionUpdateWaiter(autoPicker), championStore)
                .BeginRemoteControl();
        #endif
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _dataStore?.Save();
            _singleInstance.Release();
        }
    }
}