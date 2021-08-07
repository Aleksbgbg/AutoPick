namespace AutoPick.Converters
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;
    using AutoPick.Runes;

    public class RunePathToImageConverter : OneWayConverterBase<RunePath>
    {
        private readonly Dictionary<RunePath, BitmapImage> _images =
            Enum.GetValues<RunePath>()
                .ToDictionary(
                    runePath => runePath,
                    runePath => new BitmapImage(new Uri($"/Images/Runes/{runePath}/{runePath}.png", UriKind.Relative)));

        private protected override object ConvertValue(RunePath value)
        {
            return _images[value];
        }
    }
}