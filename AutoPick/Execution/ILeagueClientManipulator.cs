namespace AutoPick.Execution
{
    public interface ILeagueClientManipulator : ILeagueClientExecutor
    {
        void AttemptToBringLeagueToForeground();

        void RestoreLeague();
    }
}