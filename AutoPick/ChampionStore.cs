namespace AutoPick
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using System.Windows.Media.Imaging;

    public class ChampionStore
    {
        private readonly Dictionary<Champion, BitmapImage> _championImages;

        private readonly Dictionary<string, Champion> _championsByName;

        public ChampionStore()
        {
            _championImages =
                new DirectoryInfo("Champions")
                    .GetFiles()
                    .ToDictionary(
                        file => new Champion(Path.GetFileNameWithoutExtension(file.Name)),
                        file => new BitmapImage(new Uri(file.FullName, UriKind.Absolute)));
            _championsByName = _championImages.Keys.ToDictionary(champion => champion.Name, champion => champion);
        }

        public static async Task LoadChampionsIfNecessary()
        {
            using HttpClient httpClient = new()
            {
                BaseAddress = new Uri("https://iamaleks.dev/autopick/api")
            };

            if (!Directory.Exists("Champions"))
            {
                Directory.CreateDirectory("Champions");

                try
                {
                    await RegenerateImages(new HttpClientChampionLoader(httpClient));
                }
                catch (InvalidOperationException)
                {
                    await RegenerateImages(new SelfExtractingChampionLoader());
                }
                catch (HttpRequestException)
                {
                    await RegenerateImages(new SelfExtractingChampionLoader());
                }
            }
            else
            {
                using ChecksumCalculator checksumCalculator = new();

                foreach (string file in Directory.GetFiles("Champions"))
                {
                    string championName = Path.GetFileNameWithoutExtension(file);
                    byte[] imageBytes = await File.ReadAllBytesAsync(ChampionImagePath(championName));

                    checksumCalculator.Add(championName, imageBytes);
                }

                byte[] checksum = checksumCalculator.Calculate();

                try
                {
                    await VerifyImages(checksum, new HttpClientChampionLoader(httpClient));
                }
                catch (InvalidOperationException)
                {
                    await VerifyImages(checksum, new SelfExtractingChampionLoader());
                }
                catch (HttpRequestException)
                {
                    await VerifyImages(checksum, new SelfExtractingChampionLoader());
                }
            }
        }

        private static async Task VerifyImages(byte[] checksum, IChampionLoader championLoader)
        {
            byte[] loaderReportedChecksum = await championLoader.GetChecksum();

            if (checksum != loaderReportedChecksum)
            {
                if (Directory.Exists("Champions"))
                {
                    Directory.Delete("Champions", recursive: true);
                    Directory.CreateDirectory("Champions");
                }

                await RegenerateImages(championLoader);
            }
        }

        private static async Task RegenerateImages(IChampionLoader championLoader)
        {
            string[] championNames = await championLoader.GetChampionNames();

            foreach (string championName in championNames)
            {
                await using FileStream imageFile = File.Create(ChampionImagePath(championName));
                Stream imageStream = await championLoader.GetChampionImage(championName);
                await imageStream.CopyToAsync(imageFile);
            }
        }

        private static string ChampionImagePath(string championName)
        {
            return Path.Combine("Champions", $"{championName}.png");
        }

        public bool ChampionExists(string name)
        {
            return _championsByName.ContainsKey(name);
        }

        public Champion[] Champions => _championImages.Keys.ToArray();

        public BitmapImage ImageForChampion(Champion champion)
        {
            return _championImages[champion];
        }

        public Champion ChampionByName(string name)
        {
            return _championsByName[name];
        }
    }
}