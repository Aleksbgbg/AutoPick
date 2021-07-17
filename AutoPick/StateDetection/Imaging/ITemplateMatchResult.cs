namespace AutoPick.StateDetection.Imaging
{
    public interface ITemplateMatchResult
    {
        int Width { get; }

        int Height { get; }

        float this[int x, int y] { get; }
    }
}