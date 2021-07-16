namespace AutoPick.StateDetection.Definition
{
    using System;

    public static class PollingRates
    {
        public const int VerySlow = 10_000;
        public const int Slow = 2_000;
        public const int Fast = 1_000;
        public const int VeryFast = 200;
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class PollingRateAttribute : Attribute
    {
        public PollingRateAttribute(int pollingRateMs)
        {
            PollingRateMs = pollingRateMs;
        }

        public int PollingRateMs { get; }
    }
}