using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Processes;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.test.ProcessesTests;

public class SatelliteInventoryProcessTests
{
    #region GetMissingDays

    [Test]
    public async Task GetMissingDays_ReturnsDays_ValidParameters()
    {
        // Arrange
        const int year = 2021;
        var ct = CancellationToken.None;
        var business = new Mock<IDailySummaryBusiness>();
        business.Setup(s => s.GetSevereByYear(year, ct))
            .ReturnsAsync([
                    new StormEventsDailySummaryEntity { Id = "2021-07-10" },
                    new StormEventsDailySummaryEntity { Id = "2021-07-11" }
                ]
            );
        var process = new Mock<ISatelliteProcess>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteAwsInventoryListByYear(year, It.IsAny<int>(), It.IsAny<DayPartsEnum>(), ct))
            .ReturnsAsync([new SatelliteAwsInventoryEntity { EffectiveDate = "2021-07-10" }]);
        var testable = new SatelliteInventoryProcess(business.Object, process.Object, repo.Object);

        // Act
        var result = await testable.GetMissingDays(year, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0], Is.EqualTo("2021-07-11"));
        }
    }

    #endregion

    #region Run

    [Test]
    public async Task Run_UsesGoes16_OlderYear()
    {
        // Arrange
        const int year = 2022;
        const string missingDay = "2022-06-10";
        const int satellite = 16;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var ct = CancellationToken.None;
        var stormy = new Mock<IDailySummaryBusiness>();
        stormy.Setup(s => s.GetSevereByYear(year, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity() { Id = missingDay }]);
        var process = new Mock<ISatelliteProcess>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteAwsInventoryListByYear(year, channel, dayPart, ct))
            .ReturnsAsync([]);
        var testable = new SatelliteInventoryProcess(stormy.Object, process.Object, repo.Object);

        // Act
        await testable.Run(year, null!, ct);

        // Assert
        process.Verify(v => v.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct), Times.Exactly(1));
    }

    [Test]
    public async Task Run_UsesGoes19_RecentYear()
    {
        // Arrange
        const int year = 2025;
        const string missingDay = "2025-06-10";
        const int satellite = 19;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var ct = CancellationToken.None;
        var stormy = new Mock<IDailySummaryBusiness>();
        stormy.Setup(s => s.GetSevereByYear(year, ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity() { Id = missingDay }]);
        var process = new Mock<ISatelliteProcess>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteAwsInventoryListByYear(year, channel, dayPart, ct))
            .ReturnsAsync([]);
        var testable = new SatelliteInventoryProcess(stormy.Object, process.Object, repo.Object);

        // Act
        await testable.Run(year, null!, ct);

        // Assert
        process.Verify(v => v.ProcessMissingDay(year, missingDay, satellite, channel, dayPart, null!, ct), Times.Exactly(1));
    }

    #endregion

    //#region RunAsync

    //[Test]
    //public async Task RunAsync_CompletesAllSteps_ValidParameters()
    //{
    //    // Arrange
    //    const int channel = 2;
    //    const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
    //    var ct = CancellationToken.None;
    //    var stormy = new Mock<IDailySummaryBusiness>();
    //    stormy.Setup(s => s.GetSevereByYearAsync(It.IsAny<int>(), ct))
    //        .ReturnsAsync([new StormEventsDailySummaryEntity()]);
    //    var process = new Mock<ISatelliteProcess>();
    //    var source = new Mock<ISatelliteSource>();
    //    source.Setup(s => s.GetInventoryByYearAsync(It.IsAny<int>(), channel, dayPart, ct))
    //        .ReturnsAsync([]);
    //    var testable = new SatelliteInventoryProcess(stormy.Object, process.Object, source.Object);

    //    // Act
    //    await testable.RunAsync(null!, ct);

    //    // Assert
    //    source.Verify(v => v.GetInventoryByYearAsync(It.IsAny<int>(), channel, dayPart, ct), Times.Exactly(16));
    //}

    //#endregion
}
