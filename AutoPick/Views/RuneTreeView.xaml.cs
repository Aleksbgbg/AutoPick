namespace AutoPick.Views
{
    using AutoPick.Converters;

    public partial class RuneTreeView
    {
        public RuneTreeView(RunePathToImageConverter runePathToImageConverter)
        {
            Resources.Add(nameof(RunePathToImageConverter), runePathToImageConverter);

            InitializeComponent();
        }
    }
}