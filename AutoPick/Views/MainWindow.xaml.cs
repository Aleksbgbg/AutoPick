namespace AutoPick.Views
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public partial class MainWindow
    {
        public MainWindow(LaneImageFetcher laneImageFetcher)
        {
            Resources.Add("LaneImageConverter", new LaneImageConverter(laneImageFetcher));
            InitializeComponent();
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