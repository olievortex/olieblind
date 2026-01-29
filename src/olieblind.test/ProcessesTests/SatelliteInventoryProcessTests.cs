using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Enums;
using olieblind.lib.Processes;
using olieblind.lib.Satellite.Interfaces;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.test.SatelliteTests.SourcesTests;

namespace olieblind.test.ProcessesTests;

public class SatelliteInventoryProcessTests
{
    #region DownloadInventory

    [Test]
    public async Task DownloadInventory_ShortCircuit_NoKeys()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-21";
        const int satellite = 16;
        const int channel = 99;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var business = new Mock<ISatelliteImageBusiness>();
        var source = new SatelliteTestSource { Ows = null! };
        var testable = new SatelliteInventoryProcess(null!, business.Object, null!);

        // Act
        await testable.DownloadInventory(effectiveDate, satellite, channel, dayPart, source, ct);

        // Assert
        business.Verify(v => v.AddInventoryToDatabase(It.IsAny<string>(), It.IsAny<string>(),
            It.IsAny<int>(), It.IsAny<DayPartsEnum>(), ct), Times.Never);
    }

    [Test]
    public async Task DownloadInventory_Finishes_HasKeys()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-21";
        const int satellite = 16;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var business = new Mock<ISatelliteImageBusiness>();
        var source = new SatelliteTestSource { Ows = null! };
        var testable = new SatelliteInventoryProcess(null!, business.Object, null!);

        // Act
        await testable.DownloadInventory(effectiveDate, satellite, channel, dayPart, source, ct);

        // Assert
        business.Verify(v => v.AddInventoryToDatabase("2021-07-21", "", channel, dayPart, ct), Times.Once);
    }

    #endregion

    #region GetMissingDays

    [Test]
    public async Task GetMissingDays_ReturnsDays_ValidParameters()
    {
        // Arrange
        const int year = 2021;
        var ct = CancellationToken.None;
        var summaryBusiness = new Mock<IDailySummaryBusiness>();
        summaryBusiness.Setup(s => s.GetSevereByYear(year, ct))
            .ReturnsAsync([
                    new StormEventsDailySummaryEntity { Id = "2021-07-10" },
                    new StormEventsDailySummaryEntity { Id = "2021-07-11" }
                ]
            );
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteInventoryListByYear(year, It.IsAny<int>(), It.IsAny<DayPartsEnum>(), ct))
            .ReturnsAsync([new SatelliteInventoryEntity { EffectiveDate = "2021-07-10" }]);
        var imageBusiness = new Mock<ISatelliteImageBusiness>();
        var testable = new SatelliteInventoryProcess(summaryBusiness.Object, imageBusiness.Object, repo.Object);

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
    public async Task RunAsync_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        const int year = 2021;
        const int channel = 2;
        const DayPartsEnum dayPart = DayPartsEnum.Afternoon;
        var ct = CancellationToken.None;
        var stormy = new Mock<IDailySummaryBusiness>();
        stormy.Setup(s => s.GetSevereByYear(It.IsAny<int>(), ct))
            .ReturnsAsync([new StormEventsDailySummaryEntity()]);
        var business = new Mock<ISatelliteImageBusiness>();
        business.Setup(s => s.CreateSatelliteSource(year, null!))
            .Returns(new SatelliteTestSource { Ows = null! });
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SatelliteInventoryListByYear(It.IsAny<int>(), channel, dayPart, ct))
            .ReturnsAsync([]);
        var testable = new SatelliteInventoryProcess(stormy.Object, business.Object, repo.Object);

        // Act
        await testable.Run(year, null!, ct);

        // Assert
        repo.Verify(v => v.SatelliteInventoryListByYear(It.IsAny<int>(), channel, dayPart, ct), Times.Once());
    }

    #endregion

    #region SelectSatellite

    [Test]
    public async Task SelectSatellite_UsesGoes16_OlderYear()
    {
        // Arrange
        const string effectiveDate = "2022-06-10";
        const int satellite = 16;

        // Act
        var result = SatelliteInventoryProcess.SelectSatellite(effectiveDate);

        // Assert
        Assert.That(result, Is.EqualTo(satellite));
    }

    [Test]
    public async Task SelectSatellite_UsesGoes19_RecentYear()
    {
        // Arrange
        const string effectiveDate = "2025-06-10";
        const int satellite = 19;

        // Act
        var result = SatelliteInventoryProcess.SelectSatellite(effectiveDate);

        // Assert
        Assert.That(result, Is.EqualTo(satellite));
    }

    #endregion
}
