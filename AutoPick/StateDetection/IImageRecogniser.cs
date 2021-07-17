namespace AutoPick.StateDetection
{
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;

    public interface IImageRecogniser
    {
        State State { get; }

        bool IsMatch(IImage image);
    }
}