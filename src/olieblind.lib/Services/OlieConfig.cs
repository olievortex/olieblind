using Azure.Identity;
using Microsoft.Extensions.Configuration;
using System.Diagnostics.CodeAnalysis;

namespace olieblind.lib.Services;

[ExcludeFromCodeCoverage]
public class OlieConfig(IConfiguration config) : IOlieConfig
{
    private readonly IConfiguration _config = config;

    public DefaultAzureCredential Credential { get; } = new DefaultAzureCredential();

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

    public string BrownCmdPath => GetString("OlieBrownCmdPath");

    public string VideoPath => GetString("OlieVideoPath");

    public string BlobBronzeContainerUri => GetString("OlieBlobBronzeContainerUri");

    public string AwsServiceBus => GetString("OlieAwsServiceBus");

    public int SatelliteRequestGlobalLimit => GetInt("OlieSatelliteRequestGlobalLimit");

    public int SatelliteRequestUserLimit => GetInt("OlieSatelliteRequestUserLimit");

    public int SatelliteRequestLookbackHours => GetInt("OlieSatelliteRequestLookbackHours");

    public string SatelliteRequestQueueName => GetString("OlieSatelliteRequestQueueName");

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