namespace AutoPick.Execution
{
    using System;
    using System.Threading.Tasks;
    using AutoPick.Debug;
    using AutoPick.StateDetection.Definition;

    public class AutoPicker
    {
        private readonly Config _config;

        private WindowManipulator? _windowManipulator;

        private bool _enabled = true;

        private AutoPicker(Config config)
        {
            _config = config;
        }

        public static AutoPicker Run(IUserConfiguration userConfiguration, IStateConsumer stateConsumer)
        {
            AutoPicker autoPicker = new(new Config());
            Task.Factory.StartNew(async () => await autoPicker.LoopThread(userConfiguration, stateConsumer),
                                  TaskCreationOptions.LongRunning);
            return autoPicker;
        }

    #if DEBUG
        public event EventHandler? DetectionFinished;
    #endif

        public void Enable()
        {
            _enabled = true;
        }

        public void Disable()
        {
            _enabled = false;
        }

        private async Task LoopThread(IUserConfiguration userConfiguration, IStateConsumer stateConsumer)
        {
            try
            {
                PerStateActionExecutor perStateActionExecutor = new();
                perStateActionExecutor.RegisterAction(State.Accept, new AcceptStateActionExecutor());
                perStateActionExecutor.RegisterAction(State.Pick, new PickStateActionExecutor(userConfiguration));
                perStateActionExecutor.RegisterAction(State.Selected, new SelectedStateActionExecutor());
                perStateActionExecutor.RegisterAction(State.Locked, new LockedStateActionExecutor(userConfiguration));

                while (true)
                {
                    State state = _enabled ? DetectState() : State.Disabled;
                    await perStateActionExecutor.ExecuteAction(state, _windowManipulator!);
                    stateConsumer.Consume(state);
                #if DEBUG
                    DetectionFinished?.Invoke(this, EventArgs.Empty);
                #endif
                    await NextLoopDelay(state);
                }
            }
            catch (Exception e)
            {
                ErrorReporting.ReportError(e, "AutoPicker thread loop ended unexpectedly");
            }
        }

        private State DetectState()
        {
            if (WindowManipulator.HasWindow())
            {
                if (WindowManipulator.IsMinimised())
                {
                    return State.Minimised;
                }

                if (WindowManipulator.IsInvalidSize())
                {
                    return State.InvalidWindowSize;
                }

                _windowManipulator ??= WindowManipulator.Create(_config);
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

            return _windowManipulator.DetectWindowState();
        }

        private Task NextLoopDelay(State state)
        {
            return Task.Delay(_config.RefreshRates[state]);
        }
    }
}