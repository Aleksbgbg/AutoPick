namespace AutoPick.Server
{
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;

    public class MemoryChampionImageStore : IChampionStore
    {
        private Dictionary<string, byte[]> _championImages;

        public bool IsLoaded { get; private set; }

        public byte[] Checksum { get; private set; }

        public string[] ChampionNames { get; private set; }

        public byte[] ChampionImage(string championName)
        {
            return _championImages[championName];
        }

        public void ReloadChampions(Dictionary<string, byte[]> championNameToImage)
        {
            _championImages = championNameToImage;
            ChampionNames = _championImages.Keys.ToArray();
            GenerateChecksum(championNameToImage);
            IsLoaded = true;
        }

        private void GenerateChecksum(Dictionary<string, byte[]> championNameToImage)
        {
            using MemoryStream memoryStream = new();

            foreach ((string name, byte[] imageBytes) in championNameToImage)
            {
                byte[] nameBytes = Encoding.Unicode.GetBytes(name);
                memoryStream.Write(nameBytes, 0, nameBytes.Length);
                memoryStream.Write(imageBytes, 0, imageBytes.Length);
            }

            Checksum = SHA256.HashData(memoryStream.ToArray());
        }
    }
}