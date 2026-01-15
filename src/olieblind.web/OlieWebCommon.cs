using Newtonsoft.Json;

namespace olieblind.web;

public static class OlieWebCommon
{
    public const int EventsStart = 2010;

    public static async Task<T?> ApiGet<T>(IHttpClientFactory httpClientFactory, string url, CancellationToken ct) where T : class
    {
        var httpClient = Program.GetOlieBlue(httpClientFactory);
        var httpResponseMessage = await httpClient.GetAsync(url, ct);

        if (httpResponseMessage.IsSuccessStatusCode)
        {
            var body = await httpResponseMessage.Content.ReadAsStringAsync(ct);
            return JsonConvert.DeserializeObject<T>(body);
        }

        return null;
    }
}
