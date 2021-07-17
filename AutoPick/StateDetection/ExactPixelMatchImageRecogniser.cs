namespace AutoPick.StateDetection
{
    using System.Drawing;
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;

    public class ExactPixelMatchImageRecogniser : IImageRecogniser
    {
        private readonly ITemplate _template;

        private readonly Rectangle _targetLocation;

        public ExactPixelMatchImageRecogniser(State state, ITemplate template, Rectangle targetLocation)
        {
            _template = template;
            _targetLocation = targetLocation;
            State = state;
        }

        public State State { get; }

        public bool IsMatch(IImage image)
        {
            for (int x = 0; x < _targetLocation.Width; ++x)
            for (int y = 0; y < _targetLocation.Height; ++y)
            {
                IPixel sourcePixel = image[new Point(_targetLocation.X + x, _targetLocation.Y + y)];
                IPixel templatePixel = _template[new Point(x, y)];

                if (!sourcePixel.Equals(templatePixel))
                {
                    return false;
                }
            }

            return true;
        }
    }
}