namespace AutoPick
{
    using System;
    using System.Windows;
    using AutoPick.DebugTools;
    using AutoPick.Execution;
    using AutoPick.ViewModels;
    using AutoPick.Views;
    using AutoPick.WinApi;
    using AutoPick.WinApi.Native;

    public partial class App
    {
        private readonly SingleInstance _singleInstance = new();

        private readonly MainViewModel _mainViewModel = new();

        private readonly DiskDataStore _dataStore;

        public App()
        {
            _dataStore = new(_mainViewModel);
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

            MainWindow mainWindow = new()
            {
                DataContext = _mainViewModel
            };
            mainWindow.Show();

            AutoPicker autoPicker = AutoPicker.Run(_mainViewModel, _mainViewModel, _mainViewModel);

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