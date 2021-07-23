namespace AutoPick.Server
{
    public interface IChampionRetriever
    {
        bool IsLoaded { get; }

        byte[] Checksum { get; }

        string[] ChampionNames { get; }

        byte[] ChampionImage(string championName);
    }
}