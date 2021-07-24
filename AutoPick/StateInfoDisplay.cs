namespace AutoPick
{
    using AutoPick.StateDetection.Definition;

    public class StateInfoDisplay
    {
        public StateInfoDisplay(State state, InfoDisplayAttribute attribute)
        {
            State = state;
            InfoText = attribute.Text;
            Icon = attribute.Icon;
        }

        public State State { get; }

        public string InfoText { get; }

        public string Icon { get; }
    }
}