using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace olieblind.lib.Services;

[ExcludeFromCodeCoverage]
public class OlieConfig : IOlieConfig
{
    private readonly IConfiguration _config;

    public DefaultAzureCredential Credential { get; }

    public OlieConfig(IConfiguration config)
    {
        _config = config;

#pragma warning disable CS0618 // Type or member is obsolete
        var credOptions = new DefaultAzureCredentialOptions
        {
            ExcludeVisualStudioCodeCredential = true,
            ExcludeVisualStudioCredential = true,
            ExcludeSharedTokenCacheCredential = true
        };
#pragma warning restore CS0618 // Type or member is obsolete

        Credential = new DefaultAzureCredential(credOptions);
    }

    public string ApplicationInsightsConnectionString => GetString("APPLICATIONINSIGHTS_CONNECTION_STRING");

    public string BaseVideoUrl => GetString("OlieBaseVideoUrl");

    public string[] BlueCors => GetArray("OlieBlueCors");

    public string BlueUrl => GetString("OlieBlueUrl");

    public string CookieConsentCookieName => GetString("OlieCookieConsentCookieName");

    public int CookieConsentVersion => GetInt("OlieCookieConsentVersion");

    public string MySqlConnection => GetString("OlieMySqlConnection");

    public string MySqlBackupPath => GetString("OlieMySqlBackupPath");

    public Uri MySqlBackupContainer => new(GetString("OlieMySqlBackupContainer"));

    public string FfmpegCodec => GetString("OlieFfmpegCodec");

    public string FfmpegPath => GetString("OlieFfmpegPath");

    public string FontPath => GetString("OlieFontPath");

    public string SpeechRegion => GetString("OlieSpeechRegion");

    public string SpeechResourceId => GetString("OlieSpeechResourceId");

    public string ModelForecastPath => GetString("OlieModelForecastPath");

    public string SpeechVoiceName => GetString("OlieSpeechVoiceName");

    public string PurpleCmdPath => GetString("OliePurpleCmdPath");

    public string VideoPath => GetString("OlieVideoPath");

    private string GetString(string key)
    {
        return _config[key] ?? throw new ApplicationException($"{key} setting missing from configuration");
    }

    private string[] GetArray(string key)
    {
        return GetString(key).Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
    }

    private int GetInt(string key)
    {
        var raw = GetString(key);

        if (!int.TryParse(raw, out var value))
            throw new ApplicationException($"{key} must be an integer from configuration");

        return value;
    }
}