namespace AutoPick.Server.WikiDownloads
{
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using System.Threading.Tasks;
    using Microsoft.Extensions.Hosting;
    using Microsoft.Extensions.Logging;

    public class BackgroundChampionImageDownloader : IHostedService
    {
        private readonly ILogger<BackgroundChampionImageDownloader> _logger;

        private readonly IIntervalCaller _intervalCaller;

        private readonly WikiChampionDownloader _championDownloader;
        private readonly IChampionStore _championStore;

        public BackgroundChampionImageDownloader(ILogger<BackgroundChampionImageDownloader> logger,
                                                 IIntervalCaller intervalCaller,
                                                 WikiChampionDownloader championDownloader,
                                                 IChampionStore championStore)
        {
            _logger = logger;
            _intervalCaller = intervalCaller;
            _championDownloader = championDownloader;
            _championStore = championStore;
        }

        public Task StartAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(BackgroundChampionImageDownloader)} starting.");

            _intervalCaller.CallOnInterval(ReloadChampionStore, TimeSpan.Zero, TimeSpan.FromMinutes(60));

            return Task.CompletedTask;
        }

        private async void ReloadChampionStore()
        {
            _logger.LogInformation("Reloading champion images.");

            await _championDownloader.RefreshChampionSource();
            string[] names = await _championDownloader.ChampionNames();

            if (_championStore.IsLoaded && (_championStore.ChampionNames.Length == names.Length))
            {
                _logger.LogInformation("No need to reload - nothing new.");
                return;
            }

            Dictionary<string, byte[]> championNameToImage = new();

            foreach (string championName in names)
            {
                championNameToImage[championName] = await _championDownloader.ChampionImage(championName);
            }

            _championStore.ReloadChampions(championNameToImage);

            _logger.LogInformation("Champion reload finished.");
        }

        public Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation($"{nameof(BackgroundChampionImageDownloader)} stopping (no actions).");
            return Task.CompletedTask;
        }
    }
}