using Newtonsoft.Json;
using olieblind.data;
using olieblind.web.Interfaces;

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

    public static string County(string value)
    {
        value = value.Replace("county", string.Empty, StringComparison.OrdinalIgnoreCase).Trim();

        return ProperCase(value);
    }

    public static void ProcessSort(ISortableEventsPage page, string? sort)
    {
        const string county = "county";
        const string magnitude = "magnitude";
        const string state = "state";

        sort ??= string.Empty;

        page.CountyClass = page.Outline;
        page.MagnitudeClass = page.Outline;
        page.StateClass = page.Outline;
        page.TimeClass = page.Outline;

        if (sort.Equals(magnitude, StringComparison.OrdinalIgnoreCase))
        {
            page.MagnitudeClass = page.Solid;
            page.EventList = [.. page.EventList
                .OrderBy(o => o.Magnitude)
                .ThenBy(t => t.EffectiveTime)];
            return;
        }

        if (sort.Equals(state, StringComparison.OrdinalIgnoreCase))
        {
            page.StateClass = page.Solid;
            page.EventList = [.. page.EventList
                .OrderBy(o => o.State)
                .ThenBy(t => t.EffectiveTime)];
            return;
        }

        if (sort.Equals(county, StringComparison.OrdinalIgnoreCase))
        {
            page.CountyClass = page.Solid;
            page.EventList = [.. page.EventList
                .OrderBy(o => o.State)
                .ThenBy(t => t.County)
                .ThenBy(t => t.EffectiveTime)];
            return;
        }

        page.TimeClass = page.Solid;
        page.EventList = [.. page.EventList.OrderBy(t => t.EffectiveTime)];
    }

    public static string ProperCase(string value)
    {
        var parts = value.Split(' ');

        for (var i = 0; i < parts.Length; i++)
        {
            var part = parts[i];
            if (part.Length == 0) continue;

            var newPart = part[..1].ToUpper();
            if (part.Length > 1) newPart += part[1..].ToLower();
            parts[i] = newPart;
        }

        return string.Join(' ', parts);
    }

    public static string StateToAbbr(string value)
    {
        var key = OlieStates.FullToAbbr.Keys
            .SingleOrDefault(w => w.Equals(value, StringComparison.OrdinalIgnoreCase));

        return key is null ? value : OlieStates.FullToAbbr[key];
    }
}
