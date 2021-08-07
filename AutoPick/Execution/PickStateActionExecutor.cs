namespace AutoPick.Execution
{
    using System;
    using System.Buffers.Text;
    using System.IO;
    using System.Net.Http;
    using System.Net.Http.Json;
    using System.Text;
    using System.Threading.Tasks;

    public class PickStateActionExecutor : ActionExecutor
    {
        private readonly IUserConfiguration _userConfiguration;

        public PickStateActionExecutor(IUserConfiguration userConfiguration)
        {
            _userConfiguration = userConfiguration;
        }

        protected override async Task ExecuteAction(ILeagueClientExecutor clientExecutor)
        {
            await clientExecutor.CallLane(_userConfiguration.Lane);
            await clientExecutor.CallLane(_userConfiguration.Lane);
            await clientExecutor.PickChampion(_userConfiguration.ChampionName);
            await clientExecutor.CallLane(_userConfiguration.Lane);

            try
            {
                const string FilePath = "Z:\\Riot Games\\League of Legends\\lockfile";

                // string file = File.ReadAllText(FilePath);

                var s = File.Open(FilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                string file = await new StreamReader(s).ReadToEndAsync();

                string[] parts = file.Split(":");
                string port = parts[2];
                using HttpClient httpClient = new(new HttpClientHandler
                {
                    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
                })
                {
                    BaseAddress = new Uri($"https://127.0.0.1:{port}/"),
                    DefaultRequestHeaders =
                    {
                        {"Authorization",$"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"riot:{parts[3]}"))}"}
                    }
                };
                string getUrl = "/lol-perks/v1/currentpage";

                var response = await httpClient.GetAsync(getUrl);
                string res = await response.Content.ReadAsStringAsync();
                GetResponse r = (GetResponse)await response.Content.ReadFromJsonAsync(typeof(GetResponse));

                int i = r.id;
            }
            catch (Exception e)
            {
                int s = 5;
            }
        }

        private class GetResponse
        {
            public int id { get; set; }
        }
    }
}