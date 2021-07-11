namespace AutoPick.StateDetection.Definition
{
    using System;
    using System.Drawing;

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = true)]
    public class DetectorAttribute : Attribute
    {
        public DetectorAttribute(SearchAlgorithm algorithm, string templateImage, int x, int y, int width, int height)
        {
            Algorithm = algorithm;
            TemplateImage = templateImage;
            SearchLocation = new Rectangle(x, y, width, height);
        }

        public SearchAlgorithm Algorithm { get; }

        public string TemplateImage { get; }

        public Rectangle SearchLocation { get; }

        public float Threshold { get; set; } = ConvolutionImageRecogniser.DefaultThreshold;
    }
}