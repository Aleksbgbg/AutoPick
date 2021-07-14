namespace AutoPick.Execution
{
    using System.Threading.Tasks;

    public class SelectedStateActionExecutor : ActionExecutor
    {
        protected override Task ExecuteAction(ILeagueClientExecutor clientExecutor)
        {
            return clientExecutor.LockIn();
        }
    }
}