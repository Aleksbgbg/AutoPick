namespace AutoPick.Server
{
    using AutoPick.Server.WikiDownloads;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;

    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddHostedService<BackgroundChampionImageDownloader>();
            services.AddTransient<WikiChampionDownloader>();
            services.AddSingleton<ILeagueWikiDownloader, LeagueWikiDownloader>();
            services.AddSingleton<IChampionStore, MemoryChampionImageStore>();
            services.AddSingleton<IChampionRetriever>(services => services.GetService<IChampionStore>());
            services.AddSingleton<IIntervalCaller, IntervalCaller>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}