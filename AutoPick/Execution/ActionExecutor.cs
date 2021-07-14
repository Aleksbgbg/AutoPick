namespace AutoPick.Execution
{
    using System.Threading.Tasks;
    using AutoPick.StateDetection.Definition;

    public abstract class ActionExecutor
    {
        protected bool IsStateNew { get; private set; }

        public async Task Execute(State lastState, State currentState, ILeagueClientManipulator clientManipulator)
        {
            IsStateNew = lastState != currentState;

            clientManipulator.AttemptToBringLeagueToForeground();
            await ExecuteAction(clientManipulator);
            clientManipulator.RestoreLeague();
        }

        protected abstract Task ExecuteAction(ILeagueClientExecutor clientExecutor);
    }
}