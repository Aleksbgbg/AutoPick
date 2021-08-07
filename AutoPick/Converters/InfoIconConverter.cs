namespace AutoPick.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using AutoPick.StateDetection.Definition;

    public class InfoIconConverter : OneWayConverterBase<State>
    {
        private readonly Dictionary<State, BitmapImage> _imagePerState;

        public InfoIconConverter(StateInfoDisplay[] infoDisplays)
        {
            _imagePerState = infoDisplays.ToDictionary(
                display => display.State,
                display => new BitmapImage(new Uri($"/Images/Icons/{display.Icon}", UriKind.Relative)));
        }

        private protected override object ConvertValue(State value)
        {
            return _imagePerState[value];
        }
    }
}