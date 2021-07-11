namespace AutoPick
{
    using System;
    using System.Threading;
    using System.Windows;
    using AutoPick.Debug;
    using AutoPick.Execution;
    using AutoPick.ViewModels;
    using AutoPick.Views;
    using AutoPick.Win32;
    using AutoPick.Win32.Native;

    public partial class App
    {
        private readonly SingleInstance _singleInstance = new();

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

            MainViewModel mainViewModel = new();
            MainWindow mainWindow = new()
            {
                DataContext = mainViewModel
            };
            mainWindow.Show();

            AutoPicker autoPicker = AutoPicker.Run(mainViewModel, mainViewModel);

            HotKey.Factory hotKeyFactory = HotKey.Factory.For(mainWindow);

            const HotkeyModifiers modifiers =
                HotkeyModifiers.MOD_SHIFT | HotkeyModifiers.MOD_ALT | HotkeyModifiers.MOD_CONTROL;

            HotKey deactivate = hotKeyFactory.Create(modifiers, VirtualKeyCode.VK_F2);
            HotKey reactivate = hotKeyFactory.Create(modifiers, VirtualKeyCode.VK_F3);

            deactivate.Activated += (_, _) => autoPicker.Disable();
            reactivate.Activated += (_, _) => autoPicker.Enable();

#if DEBUG
            ErrorReporting.Init();
            new RemoteAppController(mainWindow, mainViewModel, new DetectionUpdateWaiter(autoPicker))
                .BeginRemoteControl();
#endif
        }

        protected override void OnExit(ExitEventArgs e)
        {
            _singleInstance.Release();
        }
    }
}