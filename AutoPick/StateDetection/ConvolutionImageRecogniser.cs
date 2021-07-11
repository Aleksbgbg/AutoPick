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
    }
}