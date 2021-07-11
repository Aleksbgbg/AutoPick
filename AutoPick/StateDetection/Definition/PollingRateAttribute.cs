namespace AutoPick.StateDetection.Definition
{
    using System;

    public static class PollingRates
    {
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