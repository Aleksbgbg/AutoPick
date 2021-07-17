namespace AutoPick.StateDetection.Imaging
{
    using System.Drawing;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    public class TemplateImage : ITemplate
    {
        private readonly Mat _template;

        public TemplateImage(Mat template)
        {
            _template = template;
        }

        public IPixel this[Point point]
            => new GrayPixel(new Gray(Utils.PixelAt(_template.DataPointer, point, _template.Width)));

        public ITemplateMatchResult Match(Mat image)
        {
            // Consider Mat implementation (not necessary)
            Image<Gray, float> output = new(image.Width - _template.Width + 1,
                                            image.Height - _template.Height + 1);
            CvInvoke.MatchTemplate(image, _template, output, TemplateMatchingType.CcoeffNormed);
            return new TemplateMatchResult(output);
        }
    }
}