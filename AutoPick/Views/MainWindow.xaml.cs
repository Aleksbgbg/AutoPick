namespace AutoPick.Views
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Reflection;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using AutoPick.StateDetection.Definition;

    public partial class MainWindow
    {
        public MainWindow(ChampionStore championStore, LaneImageFetcher laneImageFetcher)
        {
            StateInfoDisplay[] infoDisplays = Enum.GetValues<State>().Zip(typeof(State).GetMembers(BindingFlags.Static | BindingFlags.Public))
                                                           .Select(zippedValue => new StateInfoDisplay(
                                                                       zippedValue.First,
                                                                       zippedValue.Second.GetCustomAttribute<InfoDisplayAttribute>()))
                                                           .ToArray();

            Resources.Add("ChampionImageConverter", new ChampionImageConverter(championStore));
            Resources.Add("LaneImageConverter", new LaneImageConverter(laneImageFetcher));
            Resources.Add("InfoTextConverter", new InfoTextConverter(infoDisplays));
            Resources.Add("InfoIconConverter", new InfoIconConverter(infoDisplays));
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

        private class InfoTextConverter : IValueConverter
        {
            private readonly Dictionary<State, string> _infoTextPerState;

            public InfoTextConverter(StateInfoDisplay[] infoDisplays)
            {
                _infoTextPerState = infoDisplays.ToDictionary(
                    display => display.State,
                    display => display.InfoText);
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return _infoTextPerState[(State)value];
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        private class InfoIconConverter : IValueConverter
        {
            private readonly Dictionary<State, BitmapImage> _imagePerState;

            public InfoIconConverter(StateInfoDisplay[] infoDisplays)
            {
                _imagePerState = infoDisplays.ToDictionary(
                    display => display.State,
                    display => new BitmapImage(new Uri($"/Images/Icons/{display.Icon}", UriKind.Relative)));
            }

            public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            {
                return _imagePerState[(State)value];
            }

            public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }
    }
}