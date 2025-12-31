using Azure.AI.TextAnalytics;

namespace olieblind.lib.Services;

public class OlieTextSummarization(IOlieWebService ows) : IOlieTextSummarization
{
    public async Task<List<string>> SummarizeParagraphs(TextAnalyticsClient client,
        List<string> paragraphs, int sentenceLimit, bool isAbstract, CancellationToken ct)
    {
        var paragraphsToSkip = IsTooShort(paragraphs, sentenceLimit);
        if (paragraphsToSkip.All(a => a)) return paragraphs;

        var texts = ExcludeSkippedParagraphs(paragraphsToSkip, paragraphs);

        var summaries = isAbstract
            ? await ows.TextSummarizationAbstract(client, texts, sentenceLimit, ct)
            : await ows.TextSummarizationExtract(client, texts, sentenceLimit, ct);

        var flatSummaries = FlattenSummaries(summaries, sentenceLimit);
        var result = AggregateResult(paragraphsToSkip, paragraphs, flatSummaries);

        return result;
    }

    public static List<string> AggregateResult(List<bool> isSkipApi, List<string> paragraphs, string[] summaries)
    {
        if (isSkipApi.Count != paragraphs.Count)
            throw new ArgumentException("Lengths of paragraphs must be the same");
        if (isSkipApi.Count(c => !c) != summaries.Length)
            throw new ArgumentException("Summarized paragraphs length is wrong");

        var summaryIndex = 0;
        var result = isSkipApi
            .Select((t, index) => t ? paragraphs[index] : summaries[summaryIndex++])
            .ToList();

        return result;
    }

    public static List<bool> IsTooShort(List<string> paragraphs, int sentenceCount)
    {
        return [.. paragraphs
            .Select(paragraph => paragraph.Count(c => c == '.'))
            .Select(sentences => sentences < sentenceCount + 1)];
    }

    public static string[] ExcludeSkippedParagraphs(List<bool> isSkipApi, List<string> paragraphs)
    {
        if (isSkipApi.Count != paragraphs.Count)
            throw new ArgumentException("Lengths of paragraphs must be the same");

        return [.. isSkipApi.Zip(paragraphs)
            .Where(w => !w.First)
            .Select(s => s.Second)];
    }

    public static string[] FlattenSummaries(List<string[]> summaries, int limit)
    {
        var result = new List<string>();

        foreach (var summary in summaries)
        {
            result.Add(string.Join('\n', summary.Take(limit)));
        }

        return [.. result];
    }
}