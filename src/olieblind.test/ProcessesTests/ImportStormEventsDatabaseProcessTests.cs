using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Processes;
using olieblind.lib.Radar.Interfaces;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.test.ProcessesTests;

public class ImportStormEventsDatabaseProcessTests
{
    #region Run

    [Test]
    public async Task Run_AddsNew_NewData()
    {
        // Arrange
        const int year = 2021;
        const string sourceFk = "20250401";
        var ct = CancellationToken.None;
        var work = new List<DailyDetailModel>();
        for (var i = 0; i < 40; i++)
            work.Add(new DailyDetailModel { Effective = new DateTime(2021, i / 5 + 1, i % 20 + 1, 0, 0, 0, DateTimeKind.Utc) });
        var process = new Mock<IDatabaseProcess>();
        process.Setup(s => s.DeactivateOldSummaries(It.IsAny<string>(), year, sourceFk, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var databaseBusiness = new Mock<IDatabaseBusiness>();
        databaseBusiness.Setup(s => s.DatabaseLoad(null!, It.IsAny<StormEventsDatabaseEntity>(), ct))
            .ReturnsAsync(work);
        var radarBusiness = new Mock<IRadarBusiness>();
        radarBusiness.Setup(s => s.DownloadInventoryForClosestRadar(It.IsAny<List<RadarSiteEntity>>(),
                It.IsAny<List<RadarInventoryEntity>>(), It.IsAny<DateTime>(), It.IsAny<double>(), It.IsAny<double>(),
                null!, ct))
            .ReturnsAsync(new RadarSiteEntity());
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDatabaseGet(year, sourceFk, ct))
            .ReturnsAsync(new StormEventsDatabaseEntity());

        var testable = new ImportStormEventsDatabaseProcess(process.Object, databaseBusiness.Object, radarBusiness.Object, repo.Object);

        // Act
        await testable.Run(year, sourceFk, null!, null!, ct);

        // Assert
        databaseBusiness.Verify(v => v.AddDailySummaryToCosmos(It.IsAny<DailySummaryModel>(), It.IsAny<string>(), ct), Times.AtLeastOnce());
    }

    [Test]
    public async Task Run_ActivatesExisting_ExistingData()
    {
        // Arrange
        const int year = 2021;
        const string sourceFk = "20250401";
        var ct = CancellationToken.None;
        var work = new List<DailyDetailModel> { new() { Effective = new DateTime(2021, 1, 1, 12, 0, 0) } };
        var process = new Mock<IDatabaseProcess>();
        process.Setup(s => s.DeactivateOldSummaries(It.IsAny<string>(), year, sourceFk, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity { SourceFk = sourceFk }]);
        var databaseBusiness = new Mock<IDatabaseBusiness>();
        databaseBusiness.Setup(s => s.DatabaseLoad(null!, It.IsAny<StormEventsDatabaseEntity>(), ct))
            .ReturnsAsync(work);
        var radarBusiness = new Mock<IRadarBusiness>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDatabaseGet(year, sourceFk, ct))
            .ReturnsAsync(new StormEventsDatabaseEntity());
        var testable = new ImportStormEventsDatabaseProcess(process.Object, databaseBusiness.Object, radarBusiness.Object, repo.Object);

        // Act
        await testable.Run(year, sourceFk, null!, null!, ct);

        // Assert
        databaseBusiness.Verify(v => v.ActivateSummary(It.IsAny<StormEventsDailySummaryEntity>(), ct), Times.AtLeastOnce());
    }

    #endregion
}