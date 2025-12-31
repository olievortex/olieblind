using Moq;
using olieblind.lib.Processes;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;
using olieblind.lib.StormPredictionCenter.Models;
using olieblind.lib.StormPredictionCenter.Outlooks;
using olieblind.lib.Video;
using System.Drawing;

namespace olieblind.test.ProcessesTests;

public class CreateSpcOutlookVideoProcessTests
{
    private const int DayNumber = 1;

    [Test]
    public async Task Run_CreatesDatabaseRecord_ValidInput()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var ois = new Mock<IOlieImageService>();
        var outlook = new Mock<IOutlookProduct>();
        var config = new Mock<IOlieConfig>();
        var common = new Mock<ICommonProcess>();
        var parsing = new Mock<IOutlookProductParsing>();
        parsing.Setup(s => s.ParseNarrative(It.IsAny<string>(), DayNumber))
            .Returns(new OutlookProductModel());
        parsing.Setup(s => s.ExtractImageNamesFromHtml(It.IsAny<string>(), DayNumber))
            .Returns(["Dillon", "Tiffany"]);
        var scripting = new Mock<IOutlookProductScript>();
        var speech = new Mock<IOlieSpeechService>();
        var testable = new CreateSpcOutlookVideoProcess(ows.Object, ois.Object, config.Object, common.Object, outlook.Object, parsing.Object, scripting.Object, speech.Object)
        {
            FinalSize = new Point(1280, 720),
            ImageLengthSeconds = 3,
            PosterSize = new Point(320, 180)
        };

        // Act
        await testable.RunAsync(string.Empty, null!, string.Empty, string.Empty, DayNumber, ct);

        // Assert
        parsing.Verify(v => v.ParseNarrative(It.IsAny<string>(), DayNumber), Times.Once);
    }
}