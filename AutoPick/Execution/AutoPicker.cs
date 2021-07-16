namespace AutoPick.Execution
{
    using System;
    using System.Threading.Tasks;
    using AutoPick.DebugTools;
    using AutoPick.StateDetection.Definition;
    using AutoPick.WinApi.Native;

    public class AutoPicker
    {
        public const int DefaultWindowWidth = 1280;
        public const int DefaultWindowHeight = 720;

        private readonly Config _config;

        private readonly PerStateActionExecutor _actionExecutor;

        private readonly IStateConsumer _stateConsumer;
        private readonly IBitmapConsumer _bitmapConsumer;

        private WindowManipulator? _windowManipulator;

        private Win32Rect _windowSize;

        private bool _enabled = true;

        private AutoPicker(
            Config config,
            PerStateActionExecutor actionExecutor,
            IStateConsumer stateConsumer,
            IBitmapConsumer bitmapConsumer)
        {
            _config = config;
            _actionExecutor = actionExecutor;
            _stateConsumer = stateConsumer;
            _bitmapConsumer = bitmapConsumer;
        }

        public static AutoPicker Run(
            IUserConfiguration userConfiguration, IStateConsumer stateConsumer, IBitmapConsumer bitmapConsumer)
        {
            PerStateActionExecutor perStateActionExecutor = new();
            perStateActionExecutor.RegisterAction(State.Accept, new AcceptStateActionExecutor());
            perStateActionExecutor.RegisterAction(State.Pick, new PickStateActionExecutor(userConfiguration));
            perStateActionExecutor.RegisterAction(State.Selected, new SelectedStateActionExecutor());

            AutoPicker autoPicker = new(new Config(), perStateActionExecutor, stateConsumer, bitmapConsumer);
            Task.Factory.StartNew(autoPicker.LoopThread, TaskCreationOptions.LongRunning);

            return autoPicker;
        }

    #if DEBUG
        public event EventHandler? FinishedExecution;
    #endif

        public void Enable()
        {
            _enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }

        private async Task LoopThread()
        {
            try
            {
                while (true)
                {
                    State state = _enabled ? DetectState() : State.Disabled;

                    Task action = RunTakeActionThread(state);
                    RunStateReportThread(state);

                #if DEBUG
                    Task.Run(() =>
                    {
                        if (_windowManipulator != null)
                        {
                            _bitmapConsumer.Consume(_windowManipulator.LastScreenshot());
                        }
                    });
                #endif

                    await action;

                #if DEBUG
                    FinishedExecution?.Invoke(this, EventArgs.Empty);
                #endif

                    await NextLoopDelay(state);
                }
            }
            catch (Exception e)
            {
                ErrorReporting.ReportError(e, "AutoPicker thread loop ended unexpectedly");
            }
        }

        private Task RunTakeActionThread(State state)
        {
            return Task.Run(() => _actionExecutor.ExecuteAction(state, _windowManipulator!));
        }

        private void RunStateReportThread(State state)
        {
            Task.Run(() => _stateConsumer.Consume(state));
        }

        private State DetectState()
        {
            if (WindowManipulator.HasWindow(out IntPtr window))
            {
                if (WindowManipulator.IsMinimised(window))
                {
                    return State.Minimised;
                }

                if (!WindowManipulator.IsValidSize(window))
                {
                    return State.InvalidWindowSize;
                }

                _windowManipulator ??= WindowManipulator.Create(window, _config);
            }
            else
            {
                if (_windowManipulator != null)
                {
                    _windowManipulator.Delete();
                    _windowManipulator = null;
                }

                return State.NotLaunched;
            }

            Win32Rect currentWindowSize = WindowManipulator.GetWindowSize(window);

            if ((currentWindowSize.Width != _windowSize.Width) || (currentWindowSize.Height != _windowSize.Height))
            {
                _windowManipulator.Delete();
                _windowManipulator = WindowManipulator.Create(window, _config);
            }

            _windowSize = currentWindowSize;

            return _windowManipulator.DetectWindowState();
        }

        private Task NextLoopDelay(State state)
        {
            return Task.Delay(_config.RefreshRates[state]);
        }
    }
}