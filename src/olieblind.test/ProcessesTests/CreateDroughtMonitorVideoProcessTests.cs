using Moq;
using olieblind.lib.DroughtMonitor;
using olieblind.lib.DroughtMonitor.Models;
using olieblind.lib.Processes;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;
using olieblind.lib.Video;
using System.Drawing;

namespace olieblind.test.ProcessesTests;

public class CreateDroughtMonitorVideoProcessTests
{
    [Test]
    public async Task Run_NoException_ValidInput()
    {
        // Arrange
        var ows = new Mock<IOlieWebService>();
        var ct = CancellationToken.None;
        var ois = new Mock<IOlieImageService>();
        var droughtMonitor = new Mock<IDroughtMonitor>();
        droughtMonitor.Setup(s => s.GetProductFromXml(It.IsAny<string>()))
            .Returns(new DroughtMonitorProductModel());
        droughtMonitor.Setup(s => s.GetImageNames())
            .Returns(["Purple", "Blue"]);
        var scripting = new Mock<IDroughtMonitorScripting>();
        var common = new Mock<ICommonProcess>();
        var config = new Mock<IOlieConfig>();
        var speech = new Mock<IOlieSpeechService>();
        var testable = new CreateDroughtMonitorVideoProcess(ows.Object, ois.Object, config.Object, common.Object, droughtMonitor.Object, scripting.Object, speech.Object)
        {
            FinalSize = new Point(1280, 720),
            ImageLengthSeconds = 3,
            PosterSize = new Point(320, 180)
        };

        // Act
        await testable.Run(string.Empty, string.Empty, ct);

        // Verify
        droughtMonitor.Verify(v => v.GetImageNames(), Times.Once);
    }
}