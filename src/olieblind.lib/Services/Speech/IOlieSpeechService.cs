namespace olieblind.lib.Services.Speech;

public interface IOlieSpeechService
{
    Task<TimeSpan> SpeechGenerateAsync(string voiceName, string script, string fileName, CancellationToken ct);
}
