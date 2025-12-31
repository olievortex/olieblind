using Azure;
using Azure.AI.TextAnalytics;
using Moq;
using olieblind.lib.Services;

namespace olieblind.test;

public class OlieTextSummarizationTests
{
    #region SummarizeParagraphs

    [Test]
    public async Task SummarizeParagraphsAsync_CallsAbstractApi_ValidParams()
    {
        // Arrange
        const int sentenceCount = 4;
        var ct = CancellationToken.None;
        List<string> paragraphs =
        [
            "Hitchhikers. Guide. To. The. Galaxy.", "We. Are. The. News.",
            "She. baked. cookies. in. the. kitchen."
        ];
        List<string[]> apiResult = [["Chickens are white"], ["Dogs are brown"]];
        var client = new TextAnalyticsClient(new Uri("http://localhost"), new AzureKeyCredential("b"));
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.TextSummarizationAbstract(client, It.IsAny<string[]>(),
                sentenceCount, ct))
            .ReturnsAsync(apiResult);
        var testable = new OlieTextSummarization(ows.Object);

        // Act
        var result = await testable.SummarizeParagraphs(client, paragraphs, sentenceCount,
            true, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("Chickens are white"));
            Assert.That(result[1], Is.EqualTo(paragraphs[1]));
            Assert.That(result[2], Is.EqualTo("Dogs are brown"));
        }
    }

    [Test]
    public async Task SummarizeParagraphsAsync_CallsExtractApi_ValidParams()
    {
        // Arrange
        const int sentenceCount = 4;
        var ct = CancellationToken.None;
        List<string> paragraphs =
        [
            "Hitchhikers. Guide. To. The. Galaxy.", "We. Are. The. News.",
            "She. baked. cookies. in. the. kitchen."
        ];
        List<string[]> apiResult = [["Chickens are white"], ["Dogs are brown"]];
        var client = new TextAnalyticsClient(new Uri("http://localhost"), new AzureKeyCredential("b"));
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.TextSummarizationExtract(client, It.IsAny<string[]>(), sentenceCount, ct))
            .ReturnsAsync(apiResult);
        var testable = new OlieTextSummarization(ows.Object);

        // Act
        var result = await testable.SummarizeParagraphs(client, paragraphs, sentenceCount,
            false, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("Chickens are white"));
            Assert.That(result[1], Is.EqualTo(paragraphs[1]));
            Assert.That(result[2], Is.EqualTo("Dogs are brown"));
        }
    }

    [Test]
    public async Task SummarizeParagraphsAsync_ShortCircuit_ShortParagraphs()
    {
        // Arrange
        const int sentenceCount = 4;
        const string paragraph = "Fee. Fi. Fo. Fum.";
        List<string> paragraphs = [paragraph, paragraph, paragraph];
        var ows = new Mock<IOlieWebService>();
        var client = new TextAnalyticsClient(new Uri("http://localhost"), new AzureKeyCredential("b"));
        var testable = new OlieTextSummarization(ows.Object);

        // Act
        var result = await testable.SummarizeParagraphs(client, paragraphs, sentenceCount,
            true, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.EqualTo(paragraph));
            Assert.That(result[1], Is.EqualTo(paragraph));
            Assert.That(result[2], Is.EqualTo(paragraph));
        }
    }

    #endregion

    #region FlattenSummaries

    [Test]
    public void FlattenSummaries_FlattensArray_DeepSummaries()
    {
        // Arrange
        const int sentenceLimit = 3;
        string[] item1 = ["1", "2", "3"];
        string[] item2 = ["4"];
        List<string[]> data = [item1, item2];

        // Act
        var result = OlieTextSummarization.FlattenSummaries(data, sentenceLimit);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("1\n2\n3"));
            Assert.That(result[1], Is.EqualTo("4"));
        }
    }

    [Test]
    public void FlattenSummaries_Truncates_TooManySummaries()
    {
        // Arrange
        const int sentenceLimit = 2;
        string[] item1 = ["1", "2", "3"];
        string[] item2 = ["4"];
        string[] item3 = ["Dillon", "Logan"];
        List<string[]> data = [item1, item2, item3];

        // Act
        var result = OlieTextSummarization.FlattenSummaries(data, sentenceLimit);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.EqualTo("1\n2"));
            Assert.That(result[1], Is.EqualTo("4"));
            Assert.That(result[2], Is.EqualTo("Dillon\nLogan"));
        }
    }

    #endregion

    #region IsTooShort

    [Test]
    public void IsTooShort_IdentifiesSkips_ShortParagraph()
    {
        // Arrange
        const int sentenceCount = 4;
        List<string> paragraphs =
        [
            "Hitchhikers. Guide. To. The. Galaxy.", "We. Are. The. News.",
            "She. baked. cookies. in. the. kitchen."
        ];

        // Act
        var result = OlieTextSummarization.IsTooShort(paragraphs, sentenceCount);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(3));
            Assert.That(result[0], Is.False);
            Assert.That(result[1], Is.True);
            Assert.That(result[2], Is.False);
        }
    }

    #endregion

    #region ExcludeSkippedParagraphs

    [Test]
    public void ExcludeSkippedParagraphs_PrunesParagraphs_SelectedForSkip()
    {
        List<bool> isSkipApi = [true, false, false];
        List<string> paragraphs = ["1", "2", "3"];

        var result = OlieTextSummarization.ExcludeSkippedParagraphs(isSkipApi, paragraphs);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EqualTo("2"));
            Assert.That(result[1], Is.EqualTo("3"));
        }
    }

    [Test]
    public void ExcludeSkippedParagraphs_ThrowsArgumentException_MismatchedLengths()
    {
        List<bool> isSkipApi = [true, false];
        List<string> paragraphs = ["1", "2", "3"];

        Assert.Throws<ArgumentException>(() =>
            OlieTextSummarization.ExcludeSkippedParagraphs(isSkipApi, paragraphs));
    }

    #endregion

    #region AggregateResult

    [Test]
    public void AggregateResult_AggregatesSummariesAndParagraphs_Combination()
    {
        List<bool> isSkipApi = [false, true, false];
        string[] summaries = ["4", "5"];
        List<string> paragraphs = ["1", "2", "3"];

        var results = OlieTextSummarization.AggregateResult(isSkipApi, paragraphs, summaries);

        using (Assert.EnterMultipleScope())
        {
            Assert.That(results, Has.Count.EqualTo(3));
            Assert.That(results[0], Is.EqualTo("4"));
            Assert.That(results[1], Is.EqualTo("2"));
            Assert.That(results[2], Is.EqualTo("5"));
        }
    }

    [Test]
    public void AggregateResult_ThrowsArgumentException_MismatchedSummaries()
    {
        List<bool> isSkipApi = [true, false, false];
        string[] summaries = ["1", "2", "3"];
        List<string> paragraphs = ["1", "2", "3"];

        Assert.Throws<ArgumentException>(() =>
            OlieTextSummarization.AggregateResult(isSkipApi, paragraphs, summaries));
    }

    [Test]
    public void AggregateResult_ThrowsArgumentException_MismatchedParagraphs()
    {
        List<bool> isSkipApi = [false, false, false];
        string[] summaries = ["1", "2", "3"];
        List<string> paragraphs = ["1", "2"];

        Assert.Throws<ArgumentException>(() =>
            OlieTextSummarization.AggregateResult(isSkipApi, paragraphs, summaries));
    }

    #endregion
}