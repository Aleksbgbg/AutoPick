namespace AutoPick.Server.Tests
{
    using System.IO;
    using System.Linq;

    public class TestDataLoader
    {
        private TestDataLoader(string wikiPageHtml, string wikiPageExtraChampHtml, string[] championNames)
        {
            WikiPageHtml = wikiPageHtml;
            WikiPageExtraChampHtml = wikiPageExtraChampHtml;
            ChampionNames = championNames;
        }

        public static TestDataLoader Load()
        {
            string wikiPage = File.ReadAllText("TestData/wiki.html");
            string wikiPageExtraChamp = File.ReadAllText("TestData/wiki(extra champ).html");
            string[] championNames = Directory.GetFiles("TestData/ChampionImages")
                                              .Select(Path.GetFileNameWithoutExtension).ToArray();

            return new TestDataLoader(wikiPage, wikiPageExtraChamp, championNames);
        }

        public string WikiPageHtml { get; }

        public string WikiPageExtraChampHtml { get; }

        public string[] ChampionNames { get; }

        public byte[] ChampionImage(string championName)
        {
            return File.ReadAllBytes($"TestData/ChampionImages/{championName}.png");
        }
    }
}