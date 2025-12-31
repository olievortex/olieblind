namespace olieblind.lib.CookieConsent;

public class CookieConsentModel
{
    public CookieConsentStatusEnum Status { get; private set; }
    public Guid Id { get; private set; }
    public int Version { get; private set; }

    public static bool TryParse(string? cookieValue, out CookieConsentModel model)
    {
        model = new CookieConsentModel();

        // There should be a value
        if (string.IsNullOrWhiteSpace(cookieValue)) return false;

        // Separate value from cookie attributes
        cookieValue = cookieValue.Split(';')[0];

        // There should be 4 URL Encoded parts
        var parts = cookieValue.Split('&');
        if (parts.Length != 4) return false;

        // The version in part 4 should be gte current version
        if (!parts[3].StartsWith("v=")) return false;
        if (!int.TryParse(parts[3].Replace("v=", string.Empty), out var version))
            return false;

        // There should be a status in part 1
        var ptr = parts[0].IndexOf("status=", StringComparison.OrdinalIgnoreCase) + 7;
        if (ptr < 7) return false;
        var rawStatus = parts[0][ptr..];

        // Decode the status
        var status = rawStatus switch
        {
            "All" => CookieConsentStatusEnum.All,
            "Essential" => CookieConsentStatusEnum.Essential,
            _ => CookieConsentStatusEnum.Unknown
        };

        // There should be an id in part 2
        if (!parts[1].StartsWith("id=")) return false;
        if (!Guid.TryParse(parts[1].Replace("id=", string.Empty), out var id)) return false;

        // Assemble the result
        model.Id = id;
        model.Status = status;
        model.Version = version;

        return true;
    }
}