namespace AutoPick.Execution
{
    using System.Threading.Tasks;
    using AutoPick.StateDetection.Definition;

    public abstract class ActionExecutor
    {
        protected bool IsStateNew { get; private set; }

        public async Task Execute(State lastState, State currentState, WindowManipulator windowManipulator)
        {
            IsStateNew = lastState != currentState;

            windowManipulator.AttemptToBringLeagueToForeground();
            await ExecuteAction(windowManipulator);
            windowManipulator.RestoreLeague();
        }

        protected abstract Task ExecuteAction(WindowManipulator windowManipulator);
    }
}