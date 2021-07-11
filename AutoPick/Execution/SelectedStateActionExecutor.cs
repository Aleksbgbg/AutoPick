namespace AutoPick.Execution
{
    using System.Threading.Tasks;

    public class SelectedStateActionExecutor : ActionExecutor
    {
        protected override Task ExecuteAction(WindowManipulator windowManipulator)
        {
            return windowManipulator.LockIn();
        }
    }
}