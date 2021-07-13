namespace AutoPick.Tests.Unit
{
    using AutoPick.StateDetection;
    using AutoPick.StateDetection.Definition;
    using Emgu.CV;
    using Emgu.CV.Structure;
    using Xunit;

    public class ImageRecognitionTest
    {
        private readonly StateDetector _stateDetector = new(new Config());

        [Theory]
        [MemberData(nameof(TestImages.BasicScreens), MemberType = typeof(TestImages))]
        public void RecognisesBasicScreen(string image, State expectedState)
        {
            State actualState = _stateDetector.Detect(new Image<Gray, byte>(image));

            Assert.Equal(expectedState, actualState);
        }

        [Theory]
        [MemberData(nameof(TestImages.ChampSelectTransitionImages), MemberType = typeof(TestImages))]
        public void RecognisesChampSelectTransitionScreen(string image)
        {
            State actualState = _stateDetector.Detect(new Image<Gray, byte>(image));

            Assert.Equal(State.ChampSelectTransition, actualState);
        }

        [Theory]
        [MemberData(nameof(TestImages.ConnectionTransitionImages), MemberType = typeof(TestImages))]
        public void RecognisesConnectingScreen(string image)
        {
            State actualState = _stateDetector.Detect(new Image<Gray, byte>(image));

            Assert.Equal(State.Connecting, actualState);
        }
    }
}