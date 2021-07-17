namespace AutoPick.Tests.Unit
{
    using System.Drawing;
    using System.Drawing.Imaging;
    using AutoPick.StateDetection;
    using AutoPick.StateDetection.Definition;
    using AutoPick.StateDetection.Imaging;
    using Xunit;

    public class ImageRecognitionTest
    {
        private readonly StateDetector _stateDetector = new(new Config());

        [Theory]
        [MemberData(nameof(TestImages.BasicScreens), MemberType = typeof(TestImages))]
        public void RecognisesBasicScreen(string image, State expectedState)
        {
            State actualState = DetectState(image);

            Assert.Equal(expectedState, actualState);
        }

        [Theory]
        [MemberData(nameof(TestImages.ChampSelectTransitionImages), MemberType = typeof(TestImages))]
        public void RecognisesChampSelectTransitionScreen(string image)
        {
            State actualState = DetectState(image);

            Assert.Equal(State.ChampSelectTransition, actualState);
        }

        [Theory]
        [MemberData(nameof(TestImages.ConnectionTransitionImages), MemberType = typeof(TestImages))]
        public void RecognisesConnectingScreen(string image)
        {
            State actualState = DetectState(image);

            Assert.Equal(State.Connecting, actualState);
        }

        private State DetectState(string path)
        {
            using Bitmap bitmap = (Bitmap)Image.FromFile(path);
            Bitmap snapshot = new(bitmap.Width, bitmap.Height, PixelFormat.Format32bppArgb);
            using (Graphics graphics = Graphics.FromImage(snapshot)) {
                graphics.DrawImage(bitmap, new Rectangle(0, 0, snapshot.Width, snapshot.Height));
            }

            ScreenshotGenerator screenshotGenerator = new(snapshot.Size, null);
            screenshotGenerator.UpdateWindowSnapshot(snapshot);
            return _stateDetector.Detect(screenshotGenerator.RetrieveSearchImage());
        }
    }
}