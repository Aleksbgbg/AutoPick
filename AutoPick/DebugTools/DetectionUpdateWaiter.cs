#if DEBUG
namespace AutoPick.DebugTools
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using AutoPick.Execution;

    public class DetectionUpdateWaiter
    {
        private readonly AutoPicker _autoPicker;

        private readonly SemaphoreSlim _uiUpdatedSemaphore = new(0);
        private readonly SemaphoreSlim _waitingForUiUpdateSemaphore = new(0);

        public DetectionUpdateWaiter(AutoPicker autoPicker)
        {
            _autoPicker = autoPicker;
        }

        public void Start()
        {
            _autoPicker.FinishedExecution += AutoPickerOnFinishedExecution;
        }

        public Task Wait()
        {
            BeginWaitingForUiUpdate();
            return _uiUpdatedSemaphore.WaitAsync();
        }

        private void AutoPickerOnFinishedExecution(object? sender, EventArgs e)
        {
            if (IsWaitingForUiUpdate())
            {
                _waitingForUiUpdateSemaphore.Wait();
                _uiUpdatedSemaphore.Release();
            }
        }

        private void BeginWaitingForUiUpdate()
        {
            _waitingForUiUpdateSemaphore.Release();
        }

        private bool IsWaitingForUiUpdate()
        {
            return _waitingForUiUpdateSemaphore.CurrentCount > 0;
        }
    }
}
#endif