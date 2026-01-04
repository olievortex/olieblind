using Moq;
using olieblind.data.Entities;
using olieblind.lib.Processes;
using olieblind.lib.Radar.Interfaces;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.test.ProcessesTests;

public class ImportStormEventsSpcProcessTests
{
    #region ProcessStormReportsForYear

    [Test]
    public async Task Run_ShortCircuit_WhenShouldSkip()
    {
        // Arrange
        const int year = 2021;
        const int start = 42;
        const int stop = 42;
        var ct = CancellationToken.None;
        var radarBusiness = new Mock<IRadarBusiness>();
        var spc = new Mock<ISpcProcess>();
        spc.Setup(s => s.ShouldSkip(It.IsAny<StormEventsReportEntity>()))
            .Returns(true);
        spc.Setup(s => s.GetInventoryByYear(year, ct))
            .ReturnsAsync((start, stop, []));
        var testable = new ImportStormEventsSpcProcess(spc.Object, null!, radarBusiness.Object);

        // Act
        await testable.Run(year, null!, ct);

        // Assert
        spc.Verify(
            v => v.ProcessEvents(It.IsAny<List<DailyDetailModel>>(), It.IsAny<StormEventsReportEntity>(), ct),
            Times.Exactly(0));
    }

    [Test]
    public async Task Run_ShortCircuit_DateTooRecent()
    {
        // Arrange
        var now = DateTime.UtcNow;
        var year = now.Year;
        var dayNumber = (int)(now - new DateTime(now.Year, 1, 1)).TotalDays;
        var ct = CancellationToken.None;
        var radarBusiness = new Mock<IRadarBusiness>();
        var spc = new Mock<ISpcProcess>();
        spc.Setup(s => s.ShouldSkip(It.IsAny<StormEventsReportEntity>()))
            .Returns(true);
        spc.Setup(s => s.GetInventoryByYear(year, ct))
            .ReturnsAsync((dayNumber, dayNumber, []));
        var testable = new ImportStormEventsSpcProcess(spc.Object, null!, radarBusiness.Object);

        // Act
        await testable.Run(year, null!, ct);

        // Assert
        spc.Verify(
            v => v.ProcessEvents(It.IsAny<List<DailyDetailModel>>(), It.IsAny<StormEventsReportEntity>(), ct),
            Times.Exactly(0));
    }

    [Test]
    public async Task Run_CompletesAllSteps_YearToProcess()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        var radarBusiness = new Mock<IRadarBusiness>();
        radarBusiness.Setup(s => s.DownloadInventoryForClosestRadar(It.IsAny<List<RadarSiteEntity>>(),
                It.IsAny<List<RadarInventoryEntity>>(),
                It.IsAny<DateTime>(),
                It.IsAny<double>(),
                It.IsAny<double>(),
                It.IsAny<Amazon.S3.AmazonS3Client>(),
                ct))
            .ReturnsAsync(new RadarSiteEntity());
        var spc = new Mock<ISpcProcess>();
        spc.Setup(s => s.ShouldSkip(It.IsAny<StormEventsReportEntity>()))
            .Returns(false);
        spc.Setup(s => s.GetInventoryByYear(year, ct))
            .ReturnsAsync((42, 42, []));
        spc.Setup(s => s.SourceInventory(It.IsAny<DateTime>(), It.IsAny<List<StormEventsReportEntity>>(), ct))
            .ReturnsAsync(new StormEventsReportEntity());
        var spcSource = new Mock<ISpcSource>();
        spcSource.Setup(s => s.Parse(It.IsAny<DateTime>(), It.IsAny<string[]>()))
            .Returns([new()]);
        var testable = new ImportStormEventsSpcProcess(spc.Object, spcSource.Object, radarBusiness.Object);

        // Act
        await testable.Run(year, null!, ct);

        // Verify
        spc.Verify(
            v => v.ProcessEvents(It.IsAny<List<DailyDetailModel>>(), It.IsAny<StormEventsReportEntity>(), ct),
            Times.Exactly(1));
    }

    #endregion
}