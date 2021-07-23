namespace AutoPick.Server.Tests.Unit
{
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;
    using AutoPick.Server.Controllers;
    using Microsoft.AspNetCore.Mvc;
    using Xunit;

    public class ChampionFetchingTest
    {
        private readonly MemoryChampionImageStore _championImageStore;

        private readonly ChampionController _championController;

        public ChampionFetchingTest()
        {
            _championImageStore = new MemoryChampionImageStore();
            _championController = new ChampionController(_championImageStore);
        }

        [Fact]
        public void GeneratesChecksum()
        {
            Dictionary<string, byte[]> champions = new()
            {
                { "A", new byte[] { 1, 2, 3 } },
                { "B", new byte[] { 4, 5, 6 } },
                { "C", new byte[] { 7, 8, 9 } },
            };
            _championImageStore.ReloadChampions(champions);

            OkObjectResult result = _championController.GetChampionsChecksum() as OkObjectResult;

            Assert.NotNull(result);
            MemoryStream resultValue = result.Value as MemoryStream;
            // Combine all of the name + images into a byte array, then hash
            Assert.Equal(SHA256.HashData(new byte[] { 65, 0, 1, 2, 3, 66, 0, 4, 5, 6, 67, 0, 7, 8, 9 }),
                         resultValue.ToArray());
        }

        [Fact]
        public void ChampionsNotLoaded_GetChampionsChecksum_RespondsWith404()
        {
            IActionResult result = _championController.GetChampionsChecksum();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChampionsNotLoaded_GetChampions_RespondsWith404()
        {
            IActionResult result = _championController.GetChampions();

            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public void ChampionsNotLoaded_GetImage_RespondsWith404()
        {
            IActionResult result = _championController.GetImage("Ahri");

            Assert.IsType<NotFoundResult>(result);
        }
    }
}