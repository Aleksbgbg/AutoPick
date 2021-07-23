namespace AutoPick.Server.WikiDownloads
{
    using System;
    using System.Net.Http;
    using System.Threading.Tasks;
    using HtmlAgilityPack;

    public class LeagueWikiDownloader : IDisposable, ILeagueWikiDownloader
    {
        private readonly HttpClient _httpClient = new();

        public void Dispose()
        {
            _httpClient?.Dispose();
        }

        public async Task<HtmlDocument> DownloadChampionsWikiPage()
        {
            HtmlWeb htmlWeb = new();
            return await htmlWeb.LoadFromWebAsync("https://leagueoflegends.fandom.com/wiki/List_of_champions");
        }

        public async Task<byte[]> DownloadChampionImage(string url)
        {
            HttpResponseMessage response = await _httpClient.GetAsync(url);
            return await response.Content.ReadAsByteArrayAsync();
        }
    }
}