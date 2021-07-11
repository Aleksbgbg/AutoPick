namespace AutoPick
{
    using System;
    using System.Threading;

    public class SingleInstance
    {
        private Mutex? _mutex;

        public void TryConsume()
        {
            _mutex = new(true, "b5110039-4d95-42d8-8da8-8c93447cc892");
        }

        public bool InstanceAlreadyLive()
        {
            if (_mutex == null)
            {
                throw DidNotCallTryConsume();
            }

            return !_mutex.WaitOne(TimeSpan.Zero);
        }

        public void Release()
        {
            if (_mutex == null)
            {
                throw DidNotCallTryConsume();
            }

            _mutex.ReleaseMutex();
        }

        private static Exception DidNotCallTryConsume()
        {
            return new InvalidOperationException("Single instance not yet consumed (call TryConsume first)");
        }
    }
}