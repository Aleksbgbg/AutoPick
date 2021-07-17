namespace AutoPick.StateDetection
{
    using System.Linq;
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;

    public class CombinedConvolutionImageRecogniser : IImageRecogniser
    {
        private readonly ConvolutionImageRecogniser[] _recognisers;

        public CombinedConvolutionImageRecogniser(ConvolutionImageRecogniser[] recognisers)
        {
            _recognisers = recognisers;
        }

        public State State { get; private set; }

        public bool IsMatch(IImage image)
        {
            var results = _recognisers.Select(recogniser => recogniser.HighestMatch(image))
                                      .Where(result => result.SurpassesThreshold)
                                      .ToArray();

            if (results.Length == 0)
            {
                return false;
            }

            var bestMatch = results
                .Aggregate((match1, match2) => match1.HighestMatch > match2.HighestMatch ? match1 : match2);

            if (bestMatch.SurpassesThreshold)
            {
                State = bestMatch.State;
                return true;
            }

            return false;
        }
    }
}