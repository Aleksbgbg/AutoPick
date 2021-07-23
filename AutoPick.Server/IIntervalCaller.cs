namespace AutoPick.Server
{
    using System;

    public interface IIntervalCaller
    {
        void CallOnInterval(Action action, TimeSpan dueAfter, TimeSpan repeatEvery);
    }
}