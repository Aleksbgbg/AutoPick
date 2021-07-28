namespace AutoPick.Views
{
    using AutoPick.Converters;

    public partial class CalloutsView
    {
        public CalloutsView(ChampionImageConverter championImageConverter, LaneImageConverter laneImageConverter)
        {
            Resources.Add("ChampionImageConverter", championImageConverter);
            Resources.Add("LaneImageConverter", laneImageConverter);

            InitializeComponent();
        }
    }
}