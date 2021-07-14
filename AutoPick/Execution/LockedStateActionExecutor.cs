namespace AutoPick.Execution
{
    using System.Threading.Tasks;

    public class LockedStateActionExecutor : ActionExecutor
    {
        private readonly IUserConfiguration _userConfiguration;

        public LockedStateActionExecutor(IUserConfiguration userConfiguration)
        {
            _userConfiguration = userConfiguration;
        }

        protected override async Task ExecuteAction(ILeagueClientExecutor clientExecutor)
        {
            if (IsStateNew)
            {
                await clientExecutor.CallLane(_userConfiguration.LaneName);
            }
        }
    }
}