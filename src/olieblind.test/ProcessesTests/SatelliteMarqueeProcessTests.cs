using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Processes;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.Services;

namespace olieblind.test.ProcessesTests;

public class SatelliteMarqueeProcessTests
{
    #region AnnualProcess

    [Test]
    public async Task Run_ShortCircuit_NothingToDo()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        var business = new Mock<ISatelliteImageBusiness>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryListMissingPostersForYear(year, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var config = new Mock<IOlieConfig>();
        var testable = new SatelliteMarqueeProcess(business.Object, config.Object, repo.Object);

        // Act
        await testable.Run(year, null!, null!, ct);

        // Assert
        business.Verify(v => v.UpdateDailySummary1080(It.IsAny<SatelliteProductEntity>(),
            It.IsAny<StormEventsDailySummaryEntity>(), ct), Times.Never);
    }

    [Test]
    public async Task Run_ShortCircuit_1080JustCreated()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        var business = new Mock<ISatelliteImageBusiness>();
        business.Setup(s => s.GetMarqueeProduct(It.IsAny<StormEventsDailySummaryEntity>(), ct))
            .ReturnsAsync(new SatelliteProductEntity());
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryListMissingPostersForYear(year, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var config = new Mock<IOlieConfig>();
        var testable = new SatelliteMarqueeProcess(business.Object, config.Object, repo.Object);

        // Act
        await testable.Run(year, null!, null!, ct);

        // Assert
        business.Verify(v => v.UpdateDailySummary1080(It.IsAny<SatelliteProductEntity>(),
            It.IsAny<StormEventsDailySummaryEntity>(), ct), Times.Never);
    }

    [Test]
    public async Task Run_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        var business = new Mock<ISatelliteImageBusiness>();
        business.Setup(s => s.GetMarqueeProduct(It.IsAny<StormEventsDailySummaryEntity>(), ct))
            .ReturnsAsync(new SatelliteProductEntity { Path1080 = "a" });
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryListMissingPostersForYear(year, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var config = new Mock<IOlieConfig>();
        var testable = new SatelliteMarqueeProcess(business.Object, config.Object, repo.Object);

        // Act
        await testable.Run(year, null!, null!, ct);

        // Assert
        business.Verify(v => v.UpdateDailySummary1080(It.IsAny<SatelliteProductEntity>(),
            It.IsAny<StormEventsDailySummaryEntity>(), ct), Times.Exactly(1));
    }

    #endregion
}
