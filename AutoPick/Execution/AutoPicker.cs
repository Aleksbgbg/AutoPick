namespace AutoPick.Execution
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using AutoPick.DebugTools;
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;
    using AutoPick.Util;

    public class AutoPicker
    {
        public static readonly Size DefaultWindowSize = new(1280, 720);

        private readonly Config _config;

        private readonly PerStateActionExecutor _actionExecutor;

        private readonly IDetectionInfoConsumer _detectionInfoConsumer;

        private readonly ScreenshotPreviewRenderer _screenshotPreviewRenderer;

        private WindowManipulator? _windowManipulator;

        private Size _windowSize;

        private bool _enabled = true;

        private AutoPicker(
            Config config,
            PerStateActionExecutor actionExecutor,
            IDetectionInfoConsumer detectionInfoConsumer,
            ScreenshotPreviewRenderer screenshotPreviewRenderer)
        {
            _config = config;
            _actionExecutor = actionExecutor;
            _detectionInfoConsumer = detectionInfoConsumer;
            _screenshotPreviewRenderer = screenshotPreviewRenderer;
        }

        public static AutoPicker Run(
            IUserConfiguration userConfiguration,
            IDetectionInfoConsumer detectionInfoConsumer,
            ScreenshotPreviewRenderer screenshotPreviewRenderer)
        {
            PerStateActionExecutor perStateActionExecutor = new();
            perStateActionExecutor.RegisterAction(State.Accept, new AcceptStateActionExecutor());
            perStateActionExecutor.RegisterAction(State.Pick, new PickStateActionExecutor(userConfiguration));
            perStateActionExecutor.RegisterAction(State.Selected, new SelectedStateActionExecutor());

            AutoPicker autoPicker = new(new Config(), perStateActionExecutor, detectionInfoConsumer, screenshotPreviewRenderer);
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
                    RunUiUpdateThread(new DetectionInfo(state, _windowSize));
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

        private void RunUiUpdateThread(DetectionInfo detectionInfo)
        {
            Task.Run(async () =>
            {
                _detectionInfoConsumer.Consume(detectionInfo);

                await Execute.OnUiThreadAsync(() =>
                {
                    if ((_windowManipulator == null) || !detectionInfo.WindowAvailable)
                    {
                        _screenshotPreviewRenderer.Clear();
                    }
                    else
                    {
                        _windowManipulator.UpdatePreview();
                    }
                });
            });
        }

        private State DetectState()
        {
            if (!WindowManipulator.HasWindow(out IntPtr window))
            {
                if (_windowManipulator != null)
                {
                    _windowManipulator.Dispose();
                    _windowManipulator = null;
                }

                return State.NotLaunched;
            }

            if (WindowManipulator.IsMinimised(window))
            {
                return State.Minimised;
            }

            if (!WindowManipulator.IsValidSize(window))
            {
                return State.InvalidWindowSize;
            }

            Size currentWindowSize = WindowManipulator.GetWindowSize(window);

            if (_windowManipulator == null)
            {
                _windowManipulator = WindowManipulator.Create(window, _config, _screenshotPreviewRenderer);
                _windowSize = currentWindowSize;
            }
            else if (_windowSize != currentWindowSize)
            {
                _windowManipulator.Dispose();
                _windowManipulator = WindowManipulator.Create(window, _config, _screenshotPreviewRenderer);
                _windowSize = currentWindowSize;
            }

            return _windowManipulator.DetectWindowState();
        }

        private Task NextLoopDelay(State state)
        {
            return Task.Delay(_config.RefreshRates[state]);
        }
    }
}