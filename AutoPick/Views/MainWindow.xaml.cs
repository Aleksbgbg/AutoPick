namespace AutoPick.Views
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public partial class MainWindow
    {
        public MainWindow(ChampionStore championStore, LaneImageFetcher laneImageFetcher)
        {
            Resources.Add("ChampionImageConverter", new ChampionImageConverter(championStore));
            Resources.Add("LaneImageConverter", new LaneImageConverter(laneImageFetcher));
            InitializeComponent();
        }

        private class ChampionImageConverter : IValueConverter
        {
            private readonly ChampionStore _championStore;

            public ChampionImageConverter(ChampionStore championStore)
            {
                _championStore = championStore;
            }

            public object? Convert(object? value, Type targetType, object parameter, CultureInfo culture)
            {
                if (value == null || (value.GetType() != typeof(Champion)))
                {
                    return null;
                }

                return _championStore.ImageForChampion((Champion)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class LaneImageConverter : IValueConverter
        {
            private readonly LaneImageFetcher _laneImageFetcher;

            public LaneImageConverter(LaneImageFetcher laneImageFetcher)
            {
                _laneImageFetcher = laneImageFetcher;
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return _laneImageFetcher.ImageForLane((Lane)value);
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}