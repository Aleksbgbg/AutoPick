namespace AutoPick.StateDetection
{
    using System.Drawing;
    using AutoPick.StateDetection.Definition;
    using Emgu.CV;
    using Emgu.CV.Structure;

    public class ExactPixelMatchImageRecogniser : IImageRecogniser
    {
        private readonly Image<Gray, byte> _template;

        private readonly Rectangle _targetLocation;

        public ExactPixelMatchImageRecogniser(State state, Image<Gray, byte> template, Rectangle targetLocation)
        {
            _template = template;
            _targetLocation = targetLocation;
            State = state;
        }

        public State State { get; }

        public bool IsMatch(Image<Gray, byte> image)
        {
            for (int x = 0; x < _targetLocation.Width; ++x)
            for (int y = 0; y < _targetLocation.Height; ++y)
            {
                if (!image[new Point(_targetLocation.X + x, _targetLocation.Y + y)].Equals(_template[new Point(x, y)]))
                {
                    return false;
                }
            }

            return true;
        }
    }
}