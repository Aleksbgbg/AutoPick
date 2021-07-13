namespace AutoPick.Tests
{
    using System;
    using System.Drawing;
    using System.Threading.Tasks;
    using AutoPick.Win32.Native;
    using Emgu.CV;
    using Emgu.CV.CvEnum;
    using Emgu.CV.OCR;
    using Emgu.CV.Structure;
    using Xunit;

    // Utilities to help sourcing screenshots and testing features.
    // Ideally these utilities should be made into a utility program.
    // All tests here are disabled to prevent running by default.
    // All tests are parameterised with example parameters - configure for your specific use case.
    // To run a test, enable it and ensure the test case(s) use your required parameters.
    public class UtilityRunner
    {
        private const string SkipReason = "All utilities skipped by default.";

        [Theory(Skip = SkipReason)]
        [InlineData(@"A:\Downloads\tessdata-master", @"A:\Text.png", "Hi")]
        public void TestTesseract(string tesseractDataDirectory, string textImagePath, string expectedResult)
        {
            Tesseract tesseract = new(tesseractDataDirectory, "eng", OcrEngineMode.Default);
            tesseract.SetImage(new Image<Rgb, byte>(textImagePath));

            string discoveredText = tesseract.GetOsdText();

            Assert.Equal(expectedResult, discoveredText);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(@"A:\Content\League\Connecting.png",
                    @"A:\Content\League\Capture\Image1.png",
                    @"A:\Content\League\Capture\match-Image1.png",
                    32, 644, 72, 10)]
        public void MatchImageAndSaveMatchRegionToNewImage(string templatePath, string imageToMatchPath, string outputPath,
                                                           int matchX, int matchY, int matchWidth, int matchHeight)
        {
            Image<Gray, byte> template = new(templatePath);
            Image<Gray, byte> matchRegion = new Image<Gray, byte>(imageToMatchPath)
                .GetSubRect(new Rectangle(matchX, matchY, matchWidth, matchHeight));

            Image<Gray, float> matchImage = matchRegion.MatchTemplate(template, TemplateMatchingType.CcoeffNormed);
            double minVal = 0;
            double maxVal = 0;
            Point minLoc = Point.Empty;
            Point maxLoc = Point.Empty;
            CvInvoke.MinMaxLoc(matchImage, ref minVal, ref maxVal, ref minLoc, ref maxLoc);

            float[,,] matches = matchImage.Data;
            for (int y = 0; y < matches.GetLength(0); y++)
            {
                for (int x = 0; x < matches.GetLength(1); x++)
                {
                    float matchScore = matches[y, x, 0];

                    if (matchScore > 0.5f)
                    {
                        matchRegion.Draw(new Rectangle(x - 1, y - 1, matchWidth + 1, matchHeight + 1), new Gray(1));
                    }
                }

            }

            matchRegion.Save(outputPath);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(@"A:\Programming\AutoPick\AutoPick\Detect\Accept.png",
                    @"A:\Content\League\[4]Accept.png",
                    646, 383, 107, 20)]
        public void CutImage(string sourceImage, string outputPath, int matchX, int matchY, int width, int height)
        {
            Image<Gray, byte> source = new(sourceImage);
            Image<Gray, byte> target = new(new Size(width, height));

            for (int x = 0; x < width; ++x)
            {
                for (int y = 0; y < height; ++y)
                {
                    target[new Point(x, y)] = source[new Point(x + matchX, y + matchY)];
                }
            }

            target.Save(outputPath);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(0, true, @"A:\Content\League\Capture")]
        public async Task ScreenshotClient(int delayBeforeScreenshotMs, bool infinite, string outputDirectory)
        {
            IntPtr window = Win32Util.FindWindowA(null, "League of Legends");

            IntPtr sourceDeviceContext = Win32Util.GetDC(window);
            IntPtr targetDeviceContext = Win32Util.CreateCompatibleDC(sourceDeviceContext);

            Win32Util.GetWindowRect(window, out Win32Rect windowRect);
            IntPtr bitmap = Win32Util.CreateCompatibleBitmap(sourceDeviceContext, windowRect.Width, windowRect.Height);
            Win32Util.SelectObject(targetDeviceContext, bitmap);

            await Task.Delay(delayBeforeScreenshotMs);

            for (int i = 0; ; ++i)
            {
                Win32Util.PrintWindow(window, targetDeviceContext, PrintWindowParam.PW_CLIENTONLY);
                Image.FromHbitmap(bitmap).ToImage<Rgb, byte>().Save($@"{outputDirectory}\Image{i}.png");

                if (!infinite)
                {
                    break;
                }
            }

            Win32Util.DeleteObject(bitmap);
            Win32Util.DeleteDC(targetDeviceContext);
            Win32Util.ReleaseDC(window, sourceDeviceContext);
        }

        [Theory(Skip = SkipReason)]
        [InlineData(@"B:\Videos\07-04 19-55-35.avi", @"A:\Content\League\Extract")]
        public void ExtractFramesFromVideo(string videoPath, string outputDirectory)
        {
            VideoCapture videoCapture = new(videoPath, VideoCapture.API.Ffmpeg);

            for (int i = 0; ; ++i)
            {
                Mat frame = videoCapture.QueryFrame();

                if (frame == null)
                {
                    break;
                }

                frame.Save($@"{outputDirectory}\{i}.png");
            }
        }

        [Theory(Skip = SkipReason)]
        [InlineData(
            @"A:\Programming\AutoPick\AutoPick.Tests\bin\Debug\net5.0-windows\TestImages\1280x720\BasicScreens\[7]Connecting.png",
            @"A:\Programming\AutoPick\AutoPick.Tests\bin\Debug\net5.0-windows\TestImages\1920x1080\ConnectingTransitionFrames\1692.png",
            @"A:\ResizeComparison.png",
            28, 640, 80, 20)]
        public void CompareRegionBeforeAndAfterResize(string originalImagePath, string imageToResizePath,
                                                      string outputPath,
                                                      int regionX, int regionY, int regionWidth, int regionHeight)
        {
            Image<Rgb, byte> originalImage = new(originalImagePath);
            Image<Rgb, byte> imageToResize = new(imageToResizePath);
            Rectangle targetRegion = new(regionX, regionY, regionWidth, regionHeight);

            Image<Rgb, byte> comparisonOutputImage = new(targetRegion.Width, (targetRegion.Height * 2) + 1);

            Image<Rgb, byte> subregion = originalImage.GetSubRect(targetRegion);
            for (int x = 0; x < targetRegion.Width; ++x)
            {
                for (int y = 0; y < targetRegion.Height; ++y)
                {
                    comparisonOutputImage[y, x] = subregion[y, x];
                }
            }

            subregion = imageToResize.Resize(1280, 720, Inter.Lanczos4).GetSubRect(targetRegion);
            for (int y = targetRegion.Height + 1; y < ((targetRegion.Height * 2) + 1); ++y)
            {
                for (int x = 0; x < (targetRegion.Width); ++x)
                {
                    comparisonOutputImage[y, x] = subregion[y - (targetRegion.Height + 1), x];
                }
            }

            comparisonOutputImage.Draw(new LineSegment2D(new Point(0, targetRegion.Height),
                                                         new Point(targetRegion.Width, targetRegion.Height)),
                                       new Rgb(Color.Red), 1);
            comparisonOutputImage.Save(outputPath);
        }
    }
}