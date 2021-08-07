namespace AutoPick.Converters
{
    using System;
    using System.Globalization;
    using System.Windows.Data;

    public abstract class OneWayConverterBase<T> : IValueConverter
    {
        public object? Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is T tValue)
            {
                return ConvertValue(tValue);
            }

            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

        private protected abstract object? ConvertValue(T value);
    }
}