using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Processes;
using olieblind.lib.Satellite.Interfaces;
using SixLabors.ImageSharp;

namespace olieblind.test.ProcessesTests;

public class SatelliteMarqueeProcessTests
{
    #region Run

    [Test]
    public async Task Run_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        const int year = 2021;
        var ct = CancellationToken.None;
        var process = new Mock<ISatelliteImageProcess>();
        process.Setup(s => s.GetMarqueeProduct(It.IsAny<StormEventsDailySummaryEntity>(), ct))
            .ReturnsAsync(new SatelliteProductEntity());
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteProductListNoPoster(ct))
            .ReturnsAsync([new SatelliteProductEntity()]);
        repo.Setup(s => s.StormEventsDailySummaryListMissingPostersForYear(It.IsAny<int>(), ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var business = new Mock<ISatelliteImageBusiness>();
        var testable = new SatelliteMarqueeProcess(process.Object, business.Object, repo.Object);

        // Act
        await testable.Run(year, null!, null!, null!, ct);

        // Assert
        business.Verify(v => v.MakeThumbnail(It.IsAny<SatelliteProductEntity>(), It.IsAny<Point>(), null!, ct),
            Times.Once());
    }

    #endregion

    #region AnnualProcess

    [Test]
    public async Task AnnualProcess_ShortCircuit_NothingToDo()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        var process = new Mock<ISatelliteImageProcess>();
        var business = new Mock<ISatelliteImageBusiness>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryListMissingPostersForYear(year, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var testable = new SatelliteMarqueeProcess(process.Object, business.Object, repo.Object);

        // Act
        await testable.AnnualProcess(year, null!, null!, null!, ct);

        // Assert
        business.Verify(v => v.UpdateDailySummary(It.IsAny<SatelliteProductEntity>(),
            It.IsAny<StormEventsDailySummaryEntity>(), ct), Times.Never);
    }

    [Test]
    public async Task AnnualProcess_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        var process = new Mock<ISatelliteImageProcess>();
        process.Setup(s => s.GetMarqueeProduct(It.IsAny<StormEventsDailySummaryEntity>(), ct))
            .ReturnsAsync(new SatelliteProductEntity());
        var business = new Mock<ISatelliteImageBusiness>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryListMissingPostersForYear(year, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var testable = new SatelliteMarqueeProcess(process.Object, business.Object, repo.Object);

        // Act
        await testable.AnnualProcess(year, null!, null!, null!, ct);

        // Assert
        business.Verify(v => v.UpdateDailySummary(It.IsAny<SatelliteProductEntity>(),
            It.IsAny<StormEventsDailySummaryEntity>(), ct), Times.Exactly(1));
    }

    #endregion
}
