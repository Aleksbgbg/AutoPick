namespace AutoPick.StateDetection.Imaging
{
    using Emgu.CV;
    using Emgu.CV.Structure;

    public class EmguCvTemplateMatchResult : ITemplateMatchResult
    {
        private readonly Image<Gray, float> _templateMatchResult;

        public EmguCvTemplateMatchResult(Image<Gray, float> templateMatchResult)
        {
            _templateMatchResult = templateMatchResult;
        }

        public int Width => _templateMatchResult.Data.GetLength(1);

        public int Height => _templateMatchResult.Data.GetLength(0);

        public float this[int x, int y] => _templateMatchResult.Data[y, x, 0];
    }
}