namespace MockApp
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    public class ReadableAfterRelease<T> where T : new()
    {
        private const int TimeoutMs = 2000;

        private readonly SemaphoreSlim _semaphore = new(0);

        private T _value = new();

        public async Task<T> Value()
        {
            await _semaphore.WaitAsync(TimeoutMs);
            return _value;
        }

        public void Write(T value)
        {
            _value = value;
        }

        public void WriteAndRelease(T value)
        {
            _value = value;
            Release();
        }

        public void Write(Action<T> writeFunc)
        {
            writeFunc(_value);
        }

        public void WriteAndRelease(Action<T> writeFunc)
        {
            writeFunc(_value);
            Release();
        }

        private void Release()
        {
            if (_semaphore.CurrentCount == 0)
            {
                _semaphore.Release();
            }
        }
    }
}