namespace AutoPick
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Media.Imaging;

    public class LaneImageFetcher
    {
        private readonly Dictionary<Lane, BitmapImage> _laneImages =
            Enum.GetValues<Lane>()
                .ToDictionary(
                    lane => lane,
                    lane => new BitmapImage(new Uri($"/Images/Lanes/{lane.ToString()}.png", UriKind.Relative)));

        public BitmapImage ImageForLane(Lane lane)
        {
            return _laneImages[lane];
        }
    }
}