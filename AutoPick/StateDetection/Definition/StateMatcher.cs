namespace AutoPick.StateDetection.Definition
{
    public class StateMatcher
    {
        public StateMatcher(int zOrder, State state, Detector[] detectors)
        {
            ZOrder = zOrder;
            State = state;
            Detectors = detectors;
        }

        public int ZOrder { get; }

        public State State { get; }

        public Detector[] Detectors { get; }
    }
}