namespace AutoPick.StateDetection.Imaging
{
    using System.Drawing;

    public interface IImage
    {
        IPixel this[Point point] { get; }

        IImage GetSubRect(Rectangle rect);

        ITemplateMatchResult MatchTemplate(ITemplate template);
    }
}