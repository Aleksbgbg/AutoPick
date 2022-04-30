namespace AutoPick.ViewModels
{
    public class RunesViewModel : ViewModelBase
    {
        public RunesViewModel(RuneTreeViewModel[] trees)
        {
            Trees = trees;
        }

        public RuneTreeViewModel[] Trees { get; }
    }
}