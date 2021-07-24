namespace AutoPick
{
    using System;
    using System.IO;
    using System.Net;
    using System.Net.Http;
    using System.Text.Json;
    using System.Threading.Tasks;

    public class HttpClientChampionLoader : IChampionLoader
    {
        private readonly HttpClient _httpClient;

        public HttpClientChampionLoader(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.Timeout = TimeSpan.FromSeconds(1);
        }

        public async Task<byte[]> GetChecksum()
        {
            HttpResponseMessage response = await Get("champions/checksum");
            byte[] checksum = await response.Content.ReadAsByteArrayAsync();

            if (checksum.Length != 32)
            {
                throw InvalidServerResponse();
            }

            return checksum;
        }

        public async Task<string[]> GetChampionNames()
        {
            HttpResponseMessage championsResponse = await Get("champions");

            string[]? championNames;

            try
            {
                championNames = await JsonSerializer.DeserializeAsync<string[]>(
                        await championsResponse.Content.ReadAsStreamAsync());
            }
            catch (JsonException)
            {
                throw InvalidServerResponse();
            }

            if ((championNames == null) || (championNames.Length < 156))
            {
                throw InvalidServerResponse();
            }

            return championNames;
        }

        public async Task<Stream> GetChampionImage(string championName)
        {
            HttpResponseMessage imageContentResponse = await Get($"champions/{championName}/image");
            return await imageContentResponse.Content.ReadAsStreamAsync();
        }

        private async Task<HttpResponseMessage> Get(string url)
        {
            try
            {
                HttpResponseMessage response = await _httpClient.GetAsync(url);

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    throw InvalidServerResponse();
                }

                return response;
            }
            catch (TaskCanceledException)
            {
                throw InvalidServerResponse();
            }
            catch (HttpRequestException)
            {
                throw InvalidServerResponse();
            }
        }

        private Exception InvalidServerResponse()
        {
            return new InvalidOperationException("Server response cannot be verified to be accurate.");
        }
    }
}