namespace AutoPick.StateDetection.Definition
{
    public static class StateExtensions
    {
        public static bool IsWindowAvailable(this State state)
        {
            return state is not (State.NotLaunched or State.Minimised or State.InvalidWindowSize);
        }
    }
}