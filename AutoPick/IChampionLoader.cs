namespace AutoPick
{
    using System.IO;
    using System.Threading.Tasks;

    public interface IChampionLoader
    {
        Task<byte[]> GetChecksum();

        Task<string[]> GetChampionNames();

        Task<Stream> GetChampionImage(string championName);
    }
}