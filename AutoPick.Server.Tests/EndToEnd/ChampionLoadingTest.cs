namespace AutoPick.Server.Tests.EndToEnd
{
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Newtonsoft.Json;
    using Xunit;

    public class ChampionLoadingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly TestDataLoader _testDataLoader;

        private readonly HttpClient _server;

        public ChampionLoadingTest(WebApplicationFactory<Startup> webApplicationFactory)
        {
            _testDataLoader = TestDataLoader.Load();
            _server = webApplicationFactory.CreateClient();
        }

        [Fact]
        public async Task FetchChampions_LoadsImages()
        {
            HttpResponseMessage firstResponse = await _server.GetAsync("/champions");
            Assert.Equal(HttpStatusCode.NotFound, firstResponse.StatusCode);

            await Task.Delay(5_000); // Wait for champion data to be loaded from wiki

            HttpResponseMessage response = await _server.GetAsync("/champions");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string[] championNamesResult = JsonConvert.DeserializeObject<string[]>(await response.Content.ReadAsStringAsync());
            Assert.Equal(_testDataLoader.ChampionNames.Length, championNamesResult.Length);
            Assert.Equal(_testDataLoader.ChampionNames, championNamesResult);

            foreach (string championName in championNamesResult)
            {
                HttpResponseMessage imageResponse = await _server.GetAsync($"/champions/{championName}/image");

                Assert.Equal(HttpStatusCode.OK, imageResponse.StatusCode);
                byte[] expectedData = _testDataLoader.ChampionImage(championName);
                byte[] imageData = await imageResponse.Content.ReadAsByteArrayAsync();
                Assert.Equal(expectedData.Length, imageData.Length);
                Assert.Equal(expectedData, imageData);
            }
        }
    }
}