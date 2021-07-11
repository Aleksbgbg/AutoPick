namespace AutoPick.Execution
{
    using System;
    using System.Threading.Tasks;
    using AutoPick.Debug;
    using AutoPick.StateDetection.Definition;

    public class AutoPicker
    {
        private readonly Config _config;

        private readonly PerStateActionExecutor _actionExecutor;

        private readonly IStateConsumer _stateConsumer;

        private WindowManipulator? _windowManipulator;

        private bool _enabled = true;

        private AutoPicker(Config config, PerStateActionExecutor actionExecutor, IStateConsumer stateConsumer)
        {
            _config = config;
            _actionExecutor = actionExecutor;
            _stateConsumer = stateConsumer;
        }

        public static AutoPicker Run(IUserConfiguration userConfiguration, IStateConsumer stateConsumer)
        {
            PerStateActionExecutor perStateActionExecutor = new();
            perStateActionExecutor.RegisterAction(State.Accept, new AcceptStateActionExecutor());
            perStateActionExecutor.RegisterAction(State.Pick, new PickStateActionExecutor(userConfiguration));
            perStateActionExecutor.RegisterAction(State.Selected, new SelectedStateActionExecutor());
            perStateActionExecutor.RegisterAction(State.Locked, new LockedStateActionExecutor(userConfiguration));

            AutoPicker autoPicker = new(new Config(), perStateActionExecutor, stateConsumer);
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

        private Task RunStateReportThread(State state)
        {
            return Task.Run(() => _stateConsumer.Consume(state));
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