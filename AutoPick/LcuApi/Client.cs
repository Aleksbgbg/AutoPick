namespace AutoPick.LcuApi
{
    using System;
    using System.IO;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Net.Http.Json;
    using System.Text;
    using System.Text.Json;
    using System.Text.Json.Serialization;
    using System.Threading.Tasks;
    using AutoPick.Runes;
    using AutoPick.Runes.Api;

    public class Client
    {
        public static async Task<Client> Create(IRuneSelection runes)
        {
            try
            {
                const string lockfilePath = "Z:\\Riot Games\\League of Legends\\lockfile";

                await using FileStream filestream = File.Open(lockfilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                string file = await new StreamReader(filestream).ReadToEndAsync();

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

                const string currentPageUrl = "/lol-perks/v1/currentpage";

                HttpResponseMessage response = await httpClient.GetAsync(currentPageUrl);
                RunePage runePage = (RunePage)await response.Content.ReadFromJsonAsync(typeof(RunePage));

                runePage.PrimaryStyleId = runes.PrimaryRune.Id.Value;
                runePage.SecondaryStyleId = runes.SecondaryRune.Id.Value;
                runePage.SelectedPerkIds = runes.SelectedRunes.Select(r => r.Id.Value).Append(5005).Append(5008).Append(5002).ToArray();
                var s = JsonSerializer.Serialize(runePage);
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"/lol-perks/v1/pages/{runePage.Id}");
                response = await httpClient.SendAsync(request);
                request = new HttpRequestMessage(HttpMethod.Post, $"/lol-perks/v1/pages")
                {
                    Content = new StringContent(s) {Headers = { ContentType = new MediaTypeHeaderValue("application/json") }}
                };
                response = await httpClient.SendAsync(request);
            }
            catch (Exception e)
            {
            }

            return null;
        }
    }
}