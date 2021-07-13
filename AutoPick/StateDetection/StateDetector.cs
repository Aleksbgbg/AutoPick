namespace AutoPick.StateDetection
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using System.IO;
    using System.Linq;
    using System.Reflection;
    using AutoPick.Execution;
    using AutoPick.StateDetection.Definition;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.Structure;

    public class StateDetector
    {
        private readonly IImageRecogniser[] _imageRecognisers;

        public StateDetector(Config config)
        {
            _imageRecognisers = config.StateMatchers
                                      .OrderBy(matcher => matcher.ZOrder)
                                      .SelectMany(matcher => CreateImageRecogniser(matcher.State, matcher.Detectors))
                                      .ToArray();
        }

        public State Detect(Image<Gray, byte> image)
        {
            if ((image.Width != AutoPicker.DefaultWindowWidth) || (image.Height != AutoPicker.DefaultWindowHeight))
            {
                return DetectInternal(
                    image.Resize(AutoPicker.DefaultWindowWidth, AutoPicker.DefaultWindowHeight, Inter.Lanczos4));
            }

            return DetectInternal(image);
        }

        private State DetectInternal(Image<Gray, byte> image)
        {
            foreach (IImageRecogniser recogniser in _imageRecognisers)
            {
                if (recogniser.IsMatch(image))
                {
                    return recogniser.State;
                }
            }

            return State.Idle;
        }

        private static IEnumerable<IImageRecogniser> CreateImageRecogniser(State state, Detector[] detectors)
        {
            return detectors.Select(detector => CreateImageRecogniser(state, detector));
        }

        private static IImageRecogniser CreateImageRecogniser(State state, Detector detector)
        {
            return detector.SearchAlgorithm switch
            {
                SearchAlgorithm.Convolution => new ConvolutionImageRecogniser(
                    state, LoadImage(detector.TemplateImage), detector.SearchLocation, detector.Threshold),
                SearchAlgorithm.ExactPixelMatch => new ExactPixelMatchImageRecogniser(
                    state, LoadImage(detector.TemplateImage), detector.SearchLocation),
                var _ => throw new InvalidOperationException("Unimplemented search algorithm")
            };
        }

        private static Image<Gray, byte> LoadImage(string name)
        {
            using Stream? stream = Assembly.GetExecutingAssembly()
                                           .GetManifestResourceStream($"AutoPick.DetectionImages.{name}");

            if (stream == null)
            {
                throw new InvalidOperationException($"Could not find detection image {name}");
            }

            return ((Bitmap) Image.FromStream(stream)).ToImage<Gray, byte>();
        }
    }
}