namespace AutoPick.StateDetection
{
    using System.Drawing;
    using AutoPick.StateDetection.Definition;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    public class ConvolutionImageRecogniser : IImageRecogniser
    {
        public const float DefaultThreshold = 0.97f;

        private readonly Image<Gray, byte> _template;

        private readonly Rectangle _targetLocation;

        private readonly float _threshold;

        public ConvolutionImageRecogniser(State state, Image<Gray, byte> template, Rectangle targetLocation, float threshold)
        {
            _template = template;
            _targetLocation = targetLocation;
            _threshold = threshold;
            State = state;
        }

        public State State { get; }

        public bool IsMatch(Image<Gray, byte> image)
        {
            Image<Gray, float> match =  image.GetSubRect(_targetLocation)
                                             .MatchTemplate(_template, TemplateMatchingType.CcoeffNormed);

            float[,,] matches = match.Data;
            for (int y = 0; y < matches.GetLength(0); y++)
            {
                for (int x = 0; x < matches.GetLength(1); x++)
                {
                    float matchScore = matches[y, x, 0];

                    if (matchScore > _threshold)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public HighestMatchResult HighestMatch(Image<Gray, byte> image)
        {
            Image<Gray, float> match =  image.GetSubRect(_targetLocation)
                                             .MatchTemplate(_template, TemplateMatchingType.CcoeffNormed);

            float highestMatch = float.MinValue;
            bool surpassesThreshold = false;

            float[,,] matches = match.Data;
            for (int y = 0; y < matches.GetLength(0); y++)
            {
                for (int x = 0; x < matches.GetLength(1); x++)
                {
                    float matchScore = matches[y, x, 0];

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