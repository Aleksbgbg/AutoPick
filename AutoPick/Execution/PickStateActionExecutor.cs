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

        protected override async Task ExecuteAction(WindowManipulator windowManipulator)
        {
            await windowManipulator.CallLane(_userConfiguration.LaneName);
            await windowManipulator.CallLane(_userConfiguration.LaneName);
            await windowManipulator.PickChampion(_userConfiguration.ChampionName);
        }
    }
}