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

        protected override async Task ExecuteAction(WindowManipulator windowManipulator)
        {
            if (IsStateNew)
            {
                await windowManipulator.CallLane(_userConfiguration.LaneName);
            }
        }
    }
}