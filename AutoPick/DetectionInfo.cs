namespace AutoPick
{
    using System.Drawing;
    using AutoPick.StateDetection.Definition;

    public class DetectionInfo
    {
        public DetectionInfo(State state, Size windowSize)
        {
            WindowAvailable = state.IsWindowAvailable();
            State = state;
            WindowSize = windowSize;
        }

        public bool WindowAvailable { get; }

        public State State { get; }

        public Size WindowSize { get; }
    }
}