using olieblind.data;
using olieblind.data.Entities;

namespace olieblind.lib.CookieConsent;

public class CookieConsentBusiness(IMyRepository repo) : ICookieConsentBusiness
{
    public async Task<bool> LogUserCookieConsentAsync(string? cookieValue, string userAgent, string sourceIp, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(cookieValue)) return false;
        if (!CookieConsentModel.TryParse(cookieValue, out var model)) return false;

        var entity = new UserCookieConsentLogEntity
        {
            Id = model.Id.ToString("N"),
            Timestamp = DateTime.UtcNow,
            Cookie = cookieValue,
            SourceIp = sourceIp,
            Status = model.Status.ToString(),
            UserAgent = userAgent
        };

        await repo.UserCookieConsentLogCreate(entity, ct);

        return true;
    }
}