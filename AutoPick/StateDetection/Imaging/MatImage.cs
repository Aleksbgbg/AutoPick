namespace AutoPick.StateDetection.Imaging
{
    using System.Drawing;
    using Emgu.CV;
    using Emgu.CV.Structure;

    public class MatImage : IImage
    {
        private readonly Mat _mat;

        public MatImage(Mat mat)
        {
            _mat = mat;
        }

        public IPixel this[Point point]
            => new GrayPixel(new Gray(Utils.PixelAt(_mat.DataPointer, point, _mat.Width)));

        public IImage GetSubRect(Rectangle rect)
        {
            return new MatImage(new Mat(_mat, rect));
        }

        public ITemplateMatchResult MatchTemplate(ITemplate template)
        {
            return template.Match(_mat);
        }
    }
}