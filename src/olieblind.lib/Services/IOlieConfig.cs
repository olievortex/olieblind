using Azure.Identity;

namespace olieblind.lib.Services;

public interface IOlieConfig
{
    public DefaultAzureCredential Credential { get; }

    string ApplicationInsightsConnectionString { get; }

    string BaseVideoUrl { get; }

    string[] BlueCors { get; }

    string BlueUrl { get; }

    string MySqlConnection { get; }

    string CookieConsentCookieName { get; }

    int CookieConsentVersion { get; }

    string FfmpegCodec { get; }

    string FfmpegPath { get; }

    string FontPath { get; }

    string ModelForecastPath { get; }

    string MySqlBackupPath { get; }

    Uri MySqlBackupContainer { get; }

    string BrownCmdPath { get; }

    string SpeechRegion { get; }

    string SpeechResourceId { get; }

    string SpeechVoiceName { get; }

    string VideoPath { get; }

    string BlobBronzeContainerUri { get; }

    string AwsServiceBus { get; }
}
