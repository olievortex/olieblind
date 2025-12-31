namespace olieblind.lib.CookieConsent;

public interface ICookieConsentBusiness
{
    Task<bool> LogUserCookieConsentAsync(string? cookieValue, string userAgent, string sourceIp, CancellationToken ct);
}