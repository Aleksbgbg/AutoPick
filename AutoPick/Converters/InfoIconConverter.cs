namespace AutoPick.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Windows.Data;
    using System.Windows.Media.Imaging;
    using AutoPick.StateDetection.Definition;

    public class InfoIconConverter : IValueConverter
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