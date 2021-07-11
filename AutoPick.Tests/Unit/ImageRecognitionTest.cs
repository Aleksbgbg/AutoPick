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
        [InlineData("[1]Home.png", State.Idle)]
        [InlineData("[2]Lobby.png", State.Lobby)]
        [InlineData("[3]Queue.png", State.Queue)]
        [InlineData("[4]Accept.png", State.Accept)]
        [InlineData("[4]AcceptHover.png", State.Accept)]
        [InlineData("[5]Accepted.png", State.Accepted)]
        [InlineData("[5]Decline.png", State.Declined)]
        [InlineData("[6]ChampSelectTransition.png", State.ChampSelectTransition)]
        [InlineData("[6]ChampSelectTransitionBlank.png", State.ChampSelectTransition)]
        [InlineData("[7]ConnectingEarly.png", State.Connecting)]
        [InlineData("[7]Connecting.png", State.Connecting)]
        [InlineData("[8]Pick.png", State.Pick)]
        [InlineData("[9]Selected.png", State.Selected)]
        [InlineData("[9]SelectedHover.png", State.Selected)]
        [InlineData("[10]Locked.png", State.Locked)]
        [InlineData("[10]LockedHover.png", State.Locked)]
        [InlineData("[11]InGame.png", State.InGame)]
        public void RecognisesBasicScreen(string image, State expectedState)
        {
            State actualState = _stateDetector.Detect(new Image<Gray, byte>($"TestImages/1280x720/BasicScreens/{image}"));

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