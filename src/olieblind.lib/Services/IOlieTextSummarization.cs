using Azure.AI.TextAnalytics;

namespace olieblind.lib.Services;

public interface IOlieTextSummarization
{
    Task<List<string>> SummarizeParagraphs(TextAnalyticsClient client,
        List<string> paragraphs, int sentenceLimit, bool isAbstract, CancellationToken ct);
}