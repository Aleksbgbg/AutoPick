namespace AutoPick.Tests.EndToEnd
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Threading;
    using System.Threading.Tasks;

    public class TestProcessAndThreadManager : IDisposable
    {
        private readonly List<Process> _processes = new();

        private readonly List<CancellationTokenSource> _cancellationTokenSources = new();

        public void Dispose()
        {
            foreach (CancellationTokenSource cancellationTokenSource in _cancellationTokenSources)
            {
                cancellationTokenSource.Cancel();
            }

            foreach (Process process in _processes)
            {
                process.Kill();
                process.Dispose();
            }
        }

        public Process Start(string name)
        {
            Process process = Process.Start(name);
            _processes.Add(process);
            return process;
        }

        public void StartThreadWithTimeout(int timeoutMs, Action action)
        {
            CancellationTokenSource cancellationTokenSource = new();
            _cancellationTokenSources.Add(cancellationTokenSource);
            cancellationTokenSource.CancelAfter(timeoutMs);
            Task.Factory.StartNew(action,
                                  cancellationTokenSource.Token,
                                  TaskCreationOptions.LongRunning,
                                  TaskScheduler.Current);
        }
    }
}