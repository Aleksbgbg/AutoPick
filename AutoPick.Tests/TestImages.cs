namespace AutoPick.Tests
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;

    public static class TestImages
    {
        private const string ChampSelectTransitionFramesDirectory = "ChampSelectTransitionFrames";
        private const string ConnectionTransitionFramesDirectory = "ConnectingTransitionFrames";

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