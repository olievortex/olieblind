using Azure.Identity;
using Microsoft.CognitiveServices.Speech;
using Microsoft.CognitiveServices.Speech.Audio;
using System.Diagnostics.CodeAnalysis;

namespace olieblind.lib.Services.Speech;

[ExcludeFromCodeCoverage]
public class AzureSpeechService(IOlieConfig config) : IOlieSpeechService
{
    public async Task<TimeSpan> SpeechGenerateAsync(string voiceName, string script, string fileName, CancellationToken ct)
    {
        const int delay = 30;
        const int maxTries = 5;
        var retry = true;

        for (var i = 0; ; i++)
            try
            {
                return await Meow();
            }
            catch (Exception)
            {
                if (!retry || i >= maxTries) throw;
                await Task.Delay(TimeSpan.FromSeconds(delay * i), ct);
            }

        async Task<TimeSpan> Meow()
        {
            var audioConfig = AudioConfig.FromWavFileOutput(fileName);
            using var synthesizer = new SpeechSynthesizer(GetSpeechConfig(), audioConfig);
            using var result = await synthesizer.SpeakTextAsync(script);

            if (result.Reason == ResultReason.Canceled)
            {
                var cancellation = SpeechSynthesisCancellationDetails.FromResult(result);

                if (cancellation.Reason == CancellationReason.Error &&
                    cancellation.ErrorCode == CancellationErrorCode.TooManyRequests)
                    throw new ApplicationException("GenerateSpeechAsync: HTTP 429");

                retry = false;

                if (cancellation.Reason == CancellationReason.Error)
                    throw new ApplicationException(
                        $"Speech synthesizing canceled: ErrorCode={cancellation.ErrorCode}, ErrorDetails=[{cancellation.ErrorDetails}]");
                throw new ApplicationException($"Speech synthesizing canceled: Reason={cancellation.Reason}");
            }

            if (result.Reason != ResultReason.SynthesizingAudioCompleted)
            {
                retry = false;
                throw new ApplicationException($"Speech synthesizing failed. Reason={result.Reason}");
            }

            return result.AudioDuration;
        }
    }

    public SpeechConfig GetSpeechConfig()
    {
        const string SpeechSynthesisVoiceName = "en-US-NancyNeural";
        const string SpeechScope = "https://cognitiveservices.azure.com/.default";

        // Speech Config
        var context = new Azure.Core.TokenRequestContext([SpeechScope]);
        var defaultToken = new DefaultAzureCredential().GetToken(context);
        var aadToken = defaultToken.Token;
        var authorizationToken = $"aad#{config.SpeechResourceId}#{aadToken}";
        var speechConfig = SpeechConfig.FromAuthorizationToken(authorizationToken, config.SpeechRegion);
        speechConfig.SpeechSynthesisVoiceName = SpeechSynthesisVoiceName;

        return speechConfig;
    }
}
