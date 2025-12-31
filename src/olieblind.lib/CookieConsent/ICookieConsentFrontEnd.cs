namespace olieblind.lib.CookieConsent;

public interface ICookieConsentFrontEnd
{
    string CreateBaseCookie(CookieConsentStatusEnum status);
    string GetBlueApiUrl();
    CookieConsentStatusEnum GetCookieConsentStatus(string? cookieValue);
}
