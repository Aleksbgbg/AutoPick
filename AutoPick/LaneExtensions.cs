namespace AutoPick
{
    using System;

    public static class LaneExtensions
    {
        public static string ToCallout(this Lane lane)
        {
            return lane switch
            {
                Lane.Top => "top",
                Lane.Jungle => "jng",
                Lane.Mid => "mid",
                Lane.Adc => "adc",
                Lane.Support => "sup",
                _ => throw new ArgumentOutOfRangeException(nameof(lane), lane, null)
            };
        }
    }
}