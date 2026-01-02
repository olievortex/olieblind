using olieblind.lib.StormEvents.Models;

namespace olieblind.lib.StormEvents.Interfaces;

public interface ISpcSource
{
    Task<(string body, string etag)> DownloadNew(DateTime effectiveDate, CancellationToken ct);

    Task<(string body, string etag, bool isUpdated)> DownloadUpdate(DateTime effectiveDate, string etag, CancellationToken ct);

    List<DailyDetailModel> Parse(DateTime effectiveDate, string[] lines);
}