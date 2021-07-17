namespace AutoPick.StateDetection.Imaging
{
    using System.Drawing;

    public interface IImage
    {
        int Width { get; }

        int Height { get; }

        IPixel this[Point point] { get; }

        IImage Resize(int width, int height);

        IImage GetSubRect(Rectangle rect);

        ITemplateMatchResult MatchTemplate(ITemplate template);
    }
}