using Google.Cloud.TextToSpeech.V1;
using System.Diagnostics.CodeAnalysis;
using System.Text;

namespace olieblind.lib.Services.Speech;

[ExcludeFromCodeCoverage]
public class GoogleSpeechService : IOlieSpeechService
{
    public static async Task<ListVoicesResponse> ListVoices(CancellationToken ct)
    {
        var client = TextToSpeechClient.Create();
        var lvr = new ListVoicesRequest
        {
            LanguageCode = "en-US"
        };

        return await client.ListVoicesAsync(lvr, ct);
    }

    public static List<string> CreateSpeechChunks(string script, int max)
    {
        var lines = script.Replace("\r", string.Empty).Split('\n', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var result = new List<string>();
        var sb = new StringBuilder();

        foreach (var line in lines)
        {
            if ((sb.Length + line.Length) > max)
            {
                result.Add(sb.ToString());
                sb.Clear();
            }
            else
            {
                sb.AppendLine(line);
            }
        }

        if (sb.Length > 0)
        {
            result.Add(sb.ToString());
        }

        return result;
    }

    public static async Task<byte[]> SpeechGenerateChunkAsync(string voiceName, string script, CancellationToken ct)
    {
        // Instantiates a client
        var client = TextToSpeechClient.Create();

        // Set the text input to be synthesized
        var input = new SynthesisInput
        {
            Text = script
        };

        // Build the voice request, select the language code ("en-US") and the ssml voice gender
        // ("neutral")
        var voice = new VoiceSelectionParams
        {
            Name = voiceName,
            LanguageCode = "en-US"
        };

        // Select the type of audio file you want returned
        var audioConfig = new AudioConfig()
        {
            AudioEncoding = AudioEncoding.Linear16
        };

        // Perform the text-to-speech request on the text input with the selected voice parameters and
        // audio file type
        var response = await client.SynthesizeSpeechAsync(input, voice, audioConfig, ct);

        // Get the audio contents from the response
        return response.AudioContent.ToByteArray();
    }

    public static async Task<byte[]> GenerateWavAsync(string voiceName, List<string> chunks, CancellationToken ct)
    {
        var audio = new List<byte[]>();

        foreach (var chunk in chunks)
        {
            audio.Add(await SpeechGenerateChunkAsync(voiceName, chunk, ct));
        }

        var streams = audio.Select(s => (Stream)new MemoryStream(s)).ToList();
        using var dest = new MemoryStream();

        WavReader.CombineWavSources(streams, dest);

        foreach (var stream in streams)
        {
            stream.Dispose();
        }

        return dest.ToArray();
    }

    public static TimeSpan ReadWavLength(byte[] wavBytes)
    {
        using var lengthFinder = new MemoryStream(wavBytes);
        return WavReader.ReadTimeSpan(lengthFinder);
    }

    public async Task<TimeSpan> SpeechGenerateAsync(string voiceName, string script, string fileName, CancellationToken ct)
    {
        var chunks = CreateSpeechChunks(script, 4000);
        var wavBytes = await GenerateWavAsync(voiceName, chunks, ct);

        File.WriteAllBytes(fileName, wavBytes);

        var length = ReadWavLength(wavBytes);

        return length;
    }
}
