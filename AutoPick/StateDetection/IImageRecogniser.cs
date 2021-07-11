namespace AutoPick.StateDetection
{
    using AutoPick.StateDetection.Definition;
    using Emgu.CV;
    using Emgu.CV.Structure;

    public interface IImageRecogniser
    {
        State State { get; }

        bool IsMatch(Image<Gray, byte> image);
    }
}