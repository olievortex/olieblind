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
        process.SetupSequence(s => s.Download(It.IsAny<int>(), It.IsAny<int>(), string.Empty, ct))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        var source = new Mock<IMesoProductSource>();
        var testable = new SpcMesosProcess(process.Object, source.Object);

        // Act
        await testable.Run(year, false, string.Empty, ct);

        // Assert
        process.Verify(v => v.Download(It.IsAny<int>(), It.IsAny<int>(), string.Empty, ct),
            Times.Exactly(2));
    }

    [Test]
    public async Task Run_StartsFromZero_UpdateMode()
    {
        // Arrange
        const int year = 2021;
        var ct = CancellationToken.None;
        var process = new Mock<IMesoProductProcess>();
        process.SetupSequence(s => s.Download(It.IsAny<int>(), It.IsAny<int>(), string.Empty, ct))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        var source = new Mock<IMesoProductSource>();
        var testable = new SpcMesosProcess(process.Object, source.Object);

        // Act
        await testable.Run(year, true, string.Empty, ct);

        // Assert
        source.Verify(v => v.GetLatestIdForYear(year, ct), Times.Never);
    }

    #endregion

    #region DoSomething

    [Test]
    public async Task DoSomething_CompletesSteps_UpdateOnly()
    {
        // Arrange
        const int year = 2023;
        const int index = 734;
        var ct = CancellationToken.None;
        var process = new Mock<IMesoProductProcess>();
        process.SetupSequence(s => s.Download(It.IsAny<int>(), It.IsAny<int>(), null!, ct))
            .ReturnsAsync(true)
            .ReturnsAsync(false);
        var source = new Mock<IMesoProductSource>();
        var testable = new SpcMesosProcess(process.Object, source.Object);

        // Act
        await testable.DoSomething(year, index, true, null!, ct);

        // Assert
        process.Verify(v => v.Update(year, index, ct),
            Times.Exactly(1));
    }

    #endregion
}