using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.StormEvents;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.test.StormEventsTests;

public class SpcProcessTests
{
    #region GetInventoryByYear

    [Test]
    public async Task GetInventoryByYear_FetchesInventory_ValidYear()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2025;
        var spc = new Mock<ISpcBusiness>();
        var repo = new Mock<IMyRepository>();
        var testable = new SpcProcess(spc.Object, repo.Object);
        var expected = new List<StormEventsReportEntity>() { new() };
        repo.Setup(s => s.StormEventsReportsByYear(year, ct))
            .ReturnsAsync(expected);

        // Test
        var (_, __, items) = await testable.GetInventoryByYear(year, ct);

        // Assert
        Assert.That(items, Is.EqualTo(expected));
    }

    [Test]
    public async Task GetInventoryByYear_NoInventory_OldYear()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2018;
        var spc = new Mock<ISpcBusiness>();
        var repo = new Mock<IMyRepository>();
        var testable = new SpcProcess(spc.Object, repo.Object);
        repo.Setup(s => s.StormEventsReportsByYear(year, ct))
            .ReturnsAsync([new()]);

        // Test
        var (_, __, items) = await testable.GetInventoryByYear(year, ct);

        // Assert
        Assert.That(items, Is.Empty);
    }

    #endregion

    #region ProcessEvents

    [Test]
    public async Task ProcessEvents_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var business = new Mock<ISpcBusiness>();
        var testable = new SpcProcess(business.Object, null!);
        var inventory = new StormEventsReportEntity();
        var events = new List<DailyDetailModel>();

        // Act
        await testable.ProcessEvents(events, inventory, ct);

        // Assert
        business.Verify(
            v => v.AddDailyDetail(It.IsAny<List<DailyDetailModel>>(),
                It.IsAny<StormEventsReportEntity>(), ct),
            Times.Exactly(1));
    }

    #endregion

    #region ShouldSkip

    [Test]
    public void ShouldSkip_True_AlreadyComplete()
    {
        // Arrange
        var spc = new Mock<ISpcBusiness>();
        var testable = new SpcProcess(spc.Object, null!);
        var inventory = new StormEventsReportEntity
        {
            IsDailySummaryComplete = true,
            IsDailyDetailComplete = true
        };

        // Act
        var result = testable.ShouldSkip(inventory);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void ShouldSkip_False_OnlyDetailComplete()
    {
        // Arrange
        var spc = new Mock<ISpcBusiness>();
        var testable = new SpcProcess(spc.Object, null!);
        var inventory = new StormEventsReportEntity
        {
            IsDailyDetailComplete = true
        };

        // Act
        var result = testable.ShouldSkip(inventory);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region SourceInventory

    [Test]
    public async Task SourceInventoryAsync_DownloadsNew_Missing()
    {
        // Arrange
        var ct = CancellationToken.None;
        var effectiveDate = new DateTime(2010, 5, 4);
        var inventoryList = new List<StormEventsReportEntity>();
        var entity = new StormEventsReportEntity();
        var spc = new Mock<ISpcBusiness>();
        spc.Setup(s => s.GetLatest(effectiveDate, inventoryList))
            .Returns((StormEventsReportEntity?)null);
        spc.Setup(s => s.DownloadNew(effectiveDate, ct))
            .ReturnsAsync(entity);
        var testable = new SpcProcess(spc.Object, null!);

        // Act
        var result = await testable.SourceInventory(effectiveDate, inventoryList, ct);

        // Assert
        Assert.That(result, Is.EqualTo(entity));
    }

    [Test]
    public async Task SourceInventoryAsync_ChecksForUpdates_ExistingEntry()
    {
        // Arrange
        var ct = CancellationToken.None;
        var effectiveDate = new DateTime(2010, 5, 4);
        var inventoryList = new List<StormEventsReportEntity>();
        var inventory = new StormEventsReportEntity();
        var spc = new Mock<ISpcBusiness>();
        spc.Setup(s => s.DownloadUpdate(inventory, ct))
            .ReturnsAsync(inventory);
        spc.Setup(s => s.GetLatest(effectiveDate, inventoryList))
            .Returns(inventory);
        var testable = new SpcProcess(spc.Object, null!);

        // Act
        var result = await testable.SourceInventory(effectiveDate, inventoryList, ct);

        // Assert
        Assert.That(result, Is.EqualTo(inventory));
    }

    #endregion
}