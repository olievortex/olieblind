namespace olieblind.lib.Satellite.Interfaces;

public interface ISatelliteIemSource
{
    Task<List<string>> IemList(string url, CancellationToken ct);

    int GetChannelFromKey(string value);

    string GetPrefix(DateTime effectiveDate);

    DateTime GetScanTimeFromKey(DateTime effectiveDate, string value);
}