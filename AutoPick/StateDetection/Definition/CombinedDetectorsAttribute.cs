namespace AutoPick.StateDetection.Definition
{
    using System;

    [AttributeUsage(AttributeTargets.Enum)]
    public class CombinedDetectorsAttribute : Attribute
    {
        public CombinedDetectorsAttribute(params State[] states)
        {
            States = states;
        }

        public State[] States { get; }
    }
}