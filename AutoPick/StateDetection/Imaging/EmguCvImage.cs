namespace AutoPick.StateDetection.Imaging
{
    using System.Drawing;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    public class EmguCvImage : IImage, ITemplate
    {
        private readonly Image<Gray, byte> _image;

        public EmguCvImage(Image<Gray, byte> image)
        {
            _image = image;
        }

        public int Width => _image.Width;

        public int Height => _image.Height;

        public IPixel this[Point point] => new EmguCvGrayPixel(_image[point]);

        public IImage Resize(int width, int height)
        {
            return new EmguCvImage(_image.Resize(width, height, Inter.Lanczos4));
        }

        public IImage GetSubRect(Rectangle rect)
        {
            return new EmguCvImage(_image.GetSubRect(rect));
        }

        ITemplateMatchResult IImage.MatchTemplate(ITemplate template)
        {
            return template.Match(_image);
        }

        ITemplateMatchResult ITemplate.Match(Image<Gray, byte> image)
        {
            return new EmguCvTemplateMatchResult(image.MatchTemplate(_image, TemplateMatchingType.CcoeffNormed));
        }
    }
}