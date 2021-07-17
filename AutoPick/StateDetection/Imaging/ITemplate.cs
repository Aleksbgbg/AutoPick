namespace AutoPick.StateDetection.Imaging
{
    using System.Drawing;
    using Emgu.CV;
    using Emgu.CV.Structure;

    public interface ITemplate
    {
        IPixel this[Point point] { get; }

        ITemplateMatchResult Match(Image<Gray, byte> image);
    }
}