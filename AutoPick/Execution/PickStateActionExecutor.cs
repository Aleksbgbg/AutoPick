namespace AutoPick.Execution
{
    using System.Threading.Tasks;

    public class PickStateActionExecutor : ActionExecutor
    {
        private readonly IUserConfiguration _userConfiguration;

        public PickStateActionExecutor(IUserConfiguration userConfiguration)
        {
            _userConfiguration = userConfiguration;
        }

        protected override async Task ExecuteAction(ILeagueClientExecutor clientExecutor)
        {
            await clientExecutor.CallLane(_userConfiguration.LaneName);
            await clientExecutor.CallLane(_userConfiguration.LaneName);
            await clientExecutor.PickChampion(_userConfiguration.ChampionName);
            await clientExecutor.CallLane(_userConfiguration.LaneName);
        }
    }
}