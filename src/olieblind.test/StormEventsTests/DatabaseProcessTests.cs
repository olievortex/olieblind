using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.StormEvents;
using olieblind.lib.StormEvents.Interfaces;
using olieblind.lib.StormEvents.Models;

namespace olieblind.test.StormEventsTests;

public class DatabaseProcessTests
{
    #region DeactivateOldSummaries

    [Test]
    public async Task DeactivateOldSummaries_AllSteps_ValidParameters()
    {
        // Arrange
        const string id = "2021-07-18";
        const int year = 2021;
        const string sourceFk = "20250401";
        var ct = CancellationToken.None;
        var business = new Mock<IDatabaseBusiness>();
        var repo = new Mock<IMyRepository>();
        var testable = new DatabaseProcess(business.Object, repo.Object);
        var expected = new List<StormEventsDailySummaryEntity>
        {
            new() { SourceFk = sourceFk + sourceFk, IsCurrent = true },
            new() { SourceFk = sourceFk, IsCurrent = false }
        };
        repo.Setup(s => s.StormEventsDailySummaryGet(id, year, ct))
            .ReturnsAsync(expected);

        // Act
        var result = await testable.DeactivateOldSummaries(id, year, sourceFk, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region SourceDatabase

    [Test]
    public async Task SourceDatabaseAsync_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var database = new Mock<IDatabaseBusiness>();
        var repo = new Mock<IMyRepository>();
        var testable = new DatabaseProcess(database.Object, repo.Object);

        // Act
        await testable.SourceDatabases(null!, ct);

        // Assert
        database.Verify(v => v.DatabaseDownload(null!, It.IsAny<List<DatabaseFileModel>>(), ct),
            Times.Exactly(1));
    }

    #endregion
}