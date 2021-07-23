namespace AutoPick.Server.WikiDownloads
{
    using System.Threading.Tasks;
    using HtmlAgilityPack;

    public interface ILeagueWikiDownloader
    {
        Task<HtmlDocument> DownloadChampionsWikiPage();

        Task<byte[]> DownloadChampionImage(string url);
    }
}