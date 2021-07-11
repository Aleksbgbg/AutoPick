namespace AutoPick.StateDetection.Definition
{
    using System.Drawing;

    public class Detector
    {
        public Detector(DetectorAttribute detectorAttribute)
        {
            SearchAlgorithm = detectorAttribute.Algorithm;
            TemplateImage = detectorAttribute.TemplateImage;
            SearchLocation = detectorAttribute.SearchLocation;
            Threshold = detectorAttribute.Threshold;
        }

        public SearchAlgorithm SearchAlgorithm { get; }

        public string TemplateImage { get; }

        public Rectangle SearchLocation { get; }

        public float Threshold { get; }
    }
}