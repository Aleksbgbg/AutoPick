namespace AutoPick.Server
{
    using System.Collections.Generic;

    public interface IChampionStore : IChampionRetriever
    {
        void ReloadChampions(Dictionary<string, byte[]> championNameToImage);
    }
}