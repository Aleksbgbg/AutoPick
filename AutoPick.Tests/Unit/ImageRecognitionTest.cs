namespace AutoPick.Tests.Unit
{
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
            State actualState = _stateDetector.Detect(ImageFactory.ScreenshotFromPath(image));

            Assert.Equal(expectedState, actualState);
        }

        [Theory]
        [MemberData(nameof(TestImages.ChampSelectTransitionImages), MemberType = typeof(TestImages))]
        public void RecognisesChampSelectTransitionScreen(string image)
        {
            State actualState = _stateDetector.Detect(ImageFactory.ScreenshotFromPath(image));

            Assert.Equal(State.ChampSelectTransition, actualState);
        }

        [Theory]
        [MemberData(nameof(TestImages.ConnectionTransitionImages), MemberType = typeof(TestImages))]
        public void RecognisesConnectingScreen(string image)
        {
            State actualState = _stateDetector.Detect(ImageFactory.ScreenshotFromPath(image));

            Assert.Equal(State.Connecting, actualState);
        }
    }
}