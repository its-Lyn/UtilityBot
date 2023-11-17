using Newtonsoft.Json;

namespace UtilityBot.Source.NekoAPI;

public class NekoClient
{
    private const string UrlBase = "https://nekos.best/api/v2/";
    private readonly HttpClient _client = new HttpClient();

    public async Task<T> GetGenericEndpoint<T>(string endpoint)
    {
        HttpResponseMessage response = await _client.GetAsync($"{UrlBase}{endpoint}");

        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();
        return JsonConvert.DeserializeObject<T>(content) ?? throw new NullReferenceException("Received no data");
    }
}