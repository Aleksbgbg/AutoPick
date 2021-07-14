namespace AutoPick.Execution
{
    using System.Threading.Tasks;

    public class AcceptStateActionExecutor : ActionExecutor
    {
        protected override Task ExecuteAction(ILeagueClientExecutor clientExecutor)
        {
            return clientExecutor.AcceptMatch();
        }
    }
}