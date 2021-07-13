namespace AutoPick.Tests
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using AutoPick.StateDetection.Definition;

    public static class TestImages
    {
        private const string ChampSelectTransitionFramesDirectory = "ChampSelectTransitionFrames";
        private const string ConnectionTransitionFramesDirectory = "ConnectingTransitionFrames";

        public static IEnumerable<object[]> BasicScreens
        {
            get
            {
                Tuple<string, State>[] stateImages =
                {
                    new("[1]Home.png", State.Idle),
                    new("[2]Lobby.png", State.Lobby),
                    new("[3]Queue.png", State.Queue),
                    new("[4]Accept.png", State.Accept),
                    new("[4]AcceptHover.png", State.Accept),
                    new("[5]Accepted.png", State.Accepted),
                    new("[5]Decline.png", State.Declined),
                    new("[6]ChampSelectTransition.png", State.ChampSelectTransition),
                    new("[6]ChampSelectTransitionBlank.png", State.ChampSelectTransition),
                    new("[7]ConnectingEarly.png", State.Connecting),
                    new("[7]Connecting.png", State.Connecting),
                    new("[8]Pick.png", State.Pick),
                    new("[9]Selected.png", State.Selected),
                    new("[9]SelectedHover.png", State.Selected),
                    new("[10]Locked.png", State.Locked),
                    new("[10]LockedHover.png", State.Locked),
                    new("[11]InGame.png", State.InGame),
                };

                return Directory.GetDirectories("TestImages")
                                .SelectMany(directory =>
                                                stateImages.Select(stateImage => new object[]
                                                {
                                                    Path.Combine(directory, "BasicScreens", stateImage.Item1),
                                                    stateImage.Item2
                                                }))
                                .Where(test => File.Exists((string)test[0]));
            }
        }

        public static IEnumerable<object[]> ChampSelectTransitionImages =>
            FilesFrom(ChampSelectTransitionFramesDirectory);

        public static IEnumerable<object[]> ConnectionTransitionImages =>
            FilesFrom(ConnectionTransitionFramesDirectory);

        private static IEnumerable<object[]> FilesFrom(string subdirectory)
        {
            return Directory.GetDirectories("TestImages")
                            .SelectMany(directory =>
                                            Directory.GetFiles(Path.Combine(directory, subdirectory))
                                                     .Select(file => new object[] {file}));
        }
    }
}