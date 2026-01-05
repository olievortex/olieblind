using Moq;
using olieblind.lib.Processes;
using olieblind.lib.StormPredictionCenter.Interfaces;

namespace olieblind.test.ProcessesTests;

public class SpcMesosProcessTests
{
    #region Run

    [Test]
    public async Task Run_CompletesSteps_ValidParameters()
    {
        // Arrange
        const int year = 2021;
        var ct = CancellationToken.None;
        var process = new Mock<IMesoProductProcess>();
        process.SetupSequence(s => s.Download(year, It.IsAny<int>(), string.Empty, string.Empty, ct))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        var source = new Mock<IMesoProductSource>();
        var testable = new SpcMesosProcess(process.Object, source.Object);

        // Act
        await testable.Run(year, string.Empty, string.Empty, ct);

        // Assert
        process.Verify(v => v.Download(year, It.IsAny<int>(), string.Empty, string.Empty, ct),
            Times.Exactly(2));
    }

    #endregion
}