using olieblind.lib.Services;

namespace olieblind.lib.CookieConsent;

public class CookieConsentFrontEnd(IOlieConfig config) : ICookieConsentFrontEnd
{
    private readonly int _version = config.CookieConsentVersion;

    public string CreateBaseCookie(CookieConsentStatusEnum status)
    {
        var guid = Guid.NewGuid().ToString();
        var now = DateTime.UtcNow;
        var created = now.ToString("s");
        var expires = now.AddDays(180).ToString("ddd, dd MMM yyyy HH:mm:ss");

        return
            $"{config.CookieConsentCookieName}=status={status}&id={guid}&created={created}&v={_version}; Expires={expires} GMT; Path=/; SameSite=Lax; Secure";
    }

    public string GetBlueApiUrl()
    {
        return config.BlueUrl;
    }

    public CookieConsentStatusEnum GetCookieConsentStatus(string? cookieValue)
    {
        if (!CookieConsentModel.TryParse(cookieValue, out var model)) return CookieConsentStatusEnum.Unknown;

        return model.Version < _version ? CookieConsentStatusEnum.Unknown : model.Status;
    }
}
