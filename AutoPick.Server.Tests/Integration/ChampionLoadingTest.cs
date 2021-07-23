namespace AutoPick.Server.Tests.Integration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Threading.Tasks;
    using AutoPick.Server.WikiDownloads;
    using HtmlAgilityPack;
    using Microsoft.AspNetCore.Mvc.Testing;
    using Microsoft.Extensions.DependencyInjection;
    using Newtonsoft.Json;
    using Xunit;

    public class ChampionLoadingTest : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly TestDataLoader _testDataLoader;

        private readonly TestLeagueWikiDownloader _testLeagueWikiDownloader;

        private readonly TestIntervalCaller _testIntervalCaller;

        private readonly HttpClient _server;

        public ChampionLoadingTest(WebApplicationFactory<Startup> webApplicationFactory)
        {
            _testDataLoader = TestDataLoader.Load();
            _testLeagueWikiDownloader = new TestLeagueWikiDownloader(_testDataLoader);
            _testIntervalCaller = new TestIntervalCaller();
            _server = webApplicationFactory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    services.AddSingleton<ILeagueWikiDownloader>(_testLeagueWikiDownloader);
                    services.AddSingleton<IIntervalCaller>(_testIntervalCaller);
                });
            }).CreateClient();
        }

        [Fact]
        public async Task FetchChampions_LoadsImages()
        {
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

        [Fact]
        public async Task RefreshesChampionsAfter1Hour()
        {
            HttpResponseMessage response = await _server.GetAsync("/champions");

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            string[] championNamesResult = JsonConvert.DeserializeObject<string[]>(await response.Content.ReadAsStringAsync());
            Assert.Equal(_testDataLoader.ChampionNames.Length, championNamesResult.Length);

            _testLeagueWikiDownloader.AddChumiChampionWithImage(new byte[] { 255, 254, 253 });
            _testIntervalCaller.Invoke();
            await Task.Delay(1000);

            HttpResponseMessage imageResponse = await _server.GetAsync("/champions/Chumi/image");

            Assert.Equal(HttpStatusCode.OK, imageResponse.StatusCode);
            byte[] imageData = await imageResponse.Content.ReadAsByteArrayAsync();
            Assert.Equal(3, imageData.Length);
            Assert.Equal(new byte[] { 255, 254, 253 }, imageData);
            Assert.Equal(TimeSpan.FromMinutes(60), _testIntervalCaller.RepeatEvery);
        }

        private class TestLeagueWikiDownloader : ILeagueWikiDownloader
        {
            private readonly TestDataLoader _testDataLoader;

            private readonly Dictionary<string, string> _imageUrlToChampionName = new();

            private string _html;

            private bool _addedChamp;

            private byte[] _extraChampImage;

            public TestLeagueWikiDownloader(TestDataLoader testDataLoader)
            {
                _testDataLoader = testDataLoader;
                HtmlDocument document = new();
                document.LoadHtml(_testDataLoader.WikiPageHtml);
                HtmlNodeCollection nodes = document.DocumentNode.SelectNodes(
                    "/html/body/div[4]/div[2]/div[3]/main/div[3]/div[2]/div/table[2]/tbody/tr/td[1]/div/a");
                string[] names = nodes.Select(node => node.Attributes["title"].DeEntitizeValue).ToArray();
                foreach (string name in names)
                {
                    HtmlNode node = document.DocumentNode.SelectNodes(
                                                $"/html/body/div[4]/div[2]/div[3]/main/div[3]/div[2]/div/table[2]/tbody/tr/td[1]/div/a")
                                            .Single(anchor => anchor.Attributes["title"].DeEntitizeValue == name)
                                            .Element("img");
                    string imageUrl = node.Attributes["data-src"].Value.Replace("42?cb=", "50?cb=");

                    _imageUrlToChampionName.Add(imageUrl, name);
                }

                _html = testDataLoader.WikiPageHtml;
            }

            public void AddChumiChampionWithImage(byte[] data)
            {
                _html = _testDataLoader.WikiPageExtraChampHtml;
                _imageUrlToChampionName.Add(
                    "https://static.wikia.nocookie.net/leagueoflegends/images/6/6c/Chumi_OriginalSquare.png/revision/latest/scale-to-width-down/50?cb=20190430215817",
                    "Chumi");
                _extraChampImage = data;
                _addedChamp = true;
            }

            public Task<HtmlDocument> DownloadChampionsWikiPage()
            {
                HtmlDocument document = new();
                document.LoadHtml(_html);
                return Task.FromResult(document);
            }

            public Task<byte[]> DownloadChampionImage(string url)
            {
                string name = _imageUrlToChampionName[url];

                if (_addedChamp && (name == "Chumi"))
                {
                    return Task.FromResult(_extraChampImage);
                }

                return Task.FromResult(_testDataLoader.ChampionImage(name));
            }
        }

        private class TestIntervalCaller : IIntervalCaller
        {
            private Action _action;

            public TimeSpan RepeatEvery { get; private set; }

            public void Invoke()
            {
                _action();
            }

            public void CallOnInterval(Action action, TimeSpan dueAfter, TimeSpan repeatEvery)
            {
                _action = action;
                RepeatEvery = repeatEvery;

                action();
            }
        }
    }
}