namespace AutoPick.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public class ChampionImageConverter : IValueConverter
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
}