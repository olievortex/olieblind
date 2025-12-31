using olieblind.lib.Services.Speech;

namespace olieblind.cli;

public class CommandListVoices
{
    public static async Task<int> Run(CancellationToken ct)
    {
        var result = await GoogleSpeechService.ListVoices(ct);

        foreach (var voice in result.Voices)
        {
            Console.WriteLine($"{voice.Name}, {voice.SsmlGender}");
        }

        return 0;
    }
}
