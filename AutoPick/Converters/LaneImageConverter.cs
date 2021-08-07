namespace AutoPick.Converters
{
    public class LaneImageConverter : OneWayConverterBase<Lane>
    {
        private readonly LaneImageFetcher _laneImageFetcher;

        public LaneImageConverter(LaneImageFetcher laneImageFetcher)
        {
            _laneImageFetcher = laneImageFetcher;
        }

        private protected override object ConvertValue(Lane value)
        {
            return _laneImageFetcher.ImageForLane(value);
        }
    }
}