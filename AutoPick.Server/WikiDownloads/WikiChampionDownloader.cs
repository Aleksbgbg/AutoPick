namespace AutoPick.Server.WikiDownloads
{
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Threading.Tasks;
    using HtmlAgilityPack;

    public class WikiChampionDownloader
    {
        private readonly ILeagueWikiDownloader _wikiDownloader;

        private HtmlDocument _document;

        public WikiChampionDownloader(ILeagueWikiDownloader wikiDownloader)
        {
            _wikiDownloader = wikiDownloader;
        }

        public async Task RefreshChampionSource()
        {
            _document = await _wikiDownloader.DownloadChampionsWikiPage();
        }

        public Task<string[]> ChampionNames()
        {
            HtmlNodeCollection nodes = _document.DocumentNode.SelectNodes(
                "/html/body/div[4]/div[2]/div[3]/main/div[3]/div[2]/div/table[2]/tbody/tr/td[1]/div/a");
            string[] names = nodes.Select(node => node.Attributes["title"].DeEntitizeValue).ToArray();
            return Task.FromResult(names);
        }

        public async Task<byte[]> ChampionImage(string championName)
        {
            HtmlNode node = _document.DocumentNode
                                     .SelectNodes(
                                        $"/html/body/div[4]/div[2]/div[3]/main/div[3]/div[2]/div/table[2]/tbody/tr/td[1]/div/a")
                                     .Single(anchor => anchor.Attributes["title"].DeEntitizeValue == championName)
                                     .Element("img");
            string url = Regex.Replace(
                node.Attributes["data-src"].DeEntitizeValue,
                @"scale-to-width-down\/\d+(\?cb=\d+)",
                "scale-to-width-down/50$1");
            return await _wikiDownloader.DownloadChampionImage(url);
        }
    }
}