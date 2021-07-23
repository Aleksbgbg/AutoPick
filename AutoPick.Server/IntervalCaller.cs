namespace AutoPick.Server
{
    using System;
    using System.Threading;

    public class IntervalCaller : IIntervalCaller
    {
        private Timer _timer;

        public void CallOnInterval(Action action, TimeSpan dueAfter, TimeSpan repeatEvery)
        {
            _timer = new Timer(_ => action(), null, dueAfter, repeatEvery);
        }
    }
}