namespace AutoPick.StateDetection.Imaging
{
    using System.Drawing;
    using Emgu.CV;

    public interface ITemplate
    {
        IPixel this[Point point] { get; }

        ITemplateMatchResult Match(Mat image);
    }
}