namespace AutoPick.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using AutoPick.StateDetection.Definition;

    public class InfoTextConverter : IValueConverter
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
}