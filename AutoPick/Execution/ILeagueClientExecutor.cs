namespace AutoPick.Execution
{
    using System.Threading.Tasks;

    public interface ILeagueClientExecutor
    {
        Task AcceptMatch();

        Task CallLane(string lane);

        Task PickChampion(string championName);

        Task LockIn();
    }
}