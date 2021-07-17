namespace AutoPick.StateDetection
{
    using System.Drawing;
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;

    public class ConvolutionImageRecogniser : IImageRecogniser
    {
        public const float DefaultThreshold = 0.97f;

        private readonly ITemplate _template;

        private readonly Rectangle _targetLocation;

        private readonly float _threshold;

        public ConvolutionImageRecogniser(State state, ITemplate template, Rectangle targetLocation, float threshold)
        {
            _template = template;
            _targetLocation = targetLocation;
            _threshold = threshold;
            State = state;
        }

        public State State { get; }

        public bool IsMatch(IImage image)
        {
            ITemplateMatchResult match =  image.GetSubRect(_targetLocation)
                                               .MatchTemplate(_template);

            for (int x = 0; x < match.Width; ++x)
            {
                for (int y = 0; y < match.Height; ++y)
                {
                    float matchScore = match[x, y];

                    if (matchScore > _threshold)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public HighestMatchResult HighestMatch(IImage image)
        {
            ITemplateMatchResult match =  image.GetSubRect(_targetLocation)
                                               .MatchTemplate(_template);

            float highestMatch = float.MinValue;
            bool surpassesThreshold = false;

            for (int x = 0; x < match.Width; ++x)
            {
                for (int y = 0; y < match.Height; ++y)
                {
                    float matchScore = match[x, y];

                    if (matchScore > highestMatch)
                    {
                        highestMatch = matchScore;
                    }

                    if (matchScore > _threshold)
                    {
                        surpassesThreshold = true;
                    }
                }
            }

            return new HighestMatchResult(highestMatch, surpassesThreshold, State);
        }

        public readonly struct HighestMatchResult
        {
            public HighestMatchResult(float highestMatch, bool surpassesThreshold, State state)
            {
                HighestMatch = highestMatch;
                SurpassesThreshold = surpassesThreshold;
                State = state;
            }

            public float HighestMatch { get; }

            public bool SurpassesThreshold { get; }

            public State State { get; }
        }
    }
}