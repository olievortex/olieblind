using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.data.Models;
using olieblind.lib.StormEvents;

namespace olieblind.test.StormEventsTests;

public class EventsSourceTests
{
    #region FromEffectiveDate

    [Test]
    public void FromEffectiveDate_Void_Invalid()
    {
        // Arrange
        const string value = "Dillon";
        var testable = new StormEventsSource(null!);

        // Act
        var result = testable.FromEffectiveDate(value);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public void FromEffectiveDate_Date_Valid()
    {
        // Arrange
        const string value = "2021-07-18";
        var expected = new DateTime(2021, 7, 18);
        var testable = new StormEventsSource(null!);

        // Act
        var result = testable.FromEffectiveDate(value);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetAnnualSummaryList

    [Test]
    public async Task GetAnnualSummaryList_CompletesAllSteps_Valid()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        const int severeDays = 1;
        const int hailReports = 2;
        const int windReports = 3;
        const int extremeTornadoes = 4;
        const int strongTornadoes = 5;
        const int otherTornadoes = 6;
        var expected = new List<StormEventsAnnualSummaryModel>
        {
            new()
            {
                Year = year,
                SevereDays = severeDays,
                HailReports = hailReports,
                WindReports = windReports,
                ExtremeTornadoes = extremeTornadoes,
                StrongTornadoes = strongTornadoes,
                OtherTornadoes = otherTornadoes
            }
        };
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsAnnualSummaryList(ct))
            .ReturnsAsync(expected);
        var testable = new StormEventsSource(repo.Object);

        // Act
        var result = await testable.GetAnnualSummaryList(ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(1));
            Assert.That(result[0].Year, Is.EqualTo(year));
            Assert.That(result[0].SevereDays, Is.EqualTo(severeDays));
            Assert.That(result[0].HailReports, Is.EqualTo(hailReports));
            Assert.That(result[0].WindReports, Is.EqualTo(windReports));
            Assert.That(result[0].ExtremeTornadoes, Is.EqualTo(extremeTornadoes));
            Assert.That(result[0].StrongTornadoes, Is.EqualTo(strongTornadoes));
            Assert.That(result[0].OtherTornadoes, Is.EqualTo(otherTornadoes));
        }
    }

    #endregion

    #region GetDailySummaryByDate

    [Test]
    public async Task GetDailySummaryByDate_CompletesAllSteps_Valid()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-18";
        const int year = 2021;
        var a = new StormEventsDailySummaryEntity
        {
            Timestamp = DateTime.UtcNow.AddMonths(-1)
        };
        var b = new StormEventsDailySummaryEntity
        {
            Timestamp = DateTime.UtcNow
        };
        List<StormEventsDailySummaryEntity> summaries = [a, b];
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryListByDate(effectiveDate, year, ct))
            .ReturnsAsync(summaries);
        var testable = new StormEventsSource(repo.Object);

        // Act
        var result = await testable.GetDailySummaryByDate(effectiveDate, year, ct);

        // Assert
        Assert.That(result, Is.EqualTo(b));
    }

    #endregion

    #region GetDailySummaryList

    [Test]
    public async Task GetDailySummaryList_CompletesAllSteps_Valid()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2021;
        const string effectiveDate = "2021-07-18";
        var a = new StormEventsDailySummaryEntity
        {
            Id = effectiveDate,
            Timestamp = DateTime.UtcNow.AddMonths(-2)
        };
        var b = new StormEventsDailySummaryEntity
        {
            Id = effectiveDate,
            Timestamp = DateTime.UtcNow.AddMonths(-1)
        };
        var c = new StormEventsDailySummaryEntity
        {
            Id = "2021-07-19",
            Timestamp = DateTime.UtcNow
        };
        List<StormEventsDailySummaryEntity> summaries = [a, b, c];
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.StormEventsDailySummaryListByYear(year, ct))
            .ReturnsAsync(summaries);
        var testable = new StormEventsSource(repo.Object);

        // Act
        var result = await testable.GetDailySummaryList(year, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(2));
            Assert.That(result[0], Is.EqualTo(b));
            Assert.That(result[1], Is.EqualTo(c));
        }
    }

    #endregion

    #region GetIemSatelliteList

    [Test]
    public void GetIemSatelliteList_List_ValidParameters()
    {
        // Arrange
        var testable = new StormEventsSource(null!);

        // Act
        var result = testable.GetIemSatelliteList();

        // Assert
        Assert.That(result, Has.Count.EqualTo(24));
    }

    #endregion

    #region GetMeso

    [Test]
    public async Task GetMeso_AllSteps_ValidParameters()
    {
        // Arrange
        const int id = 42;
        const int year = 2021;
        const string effectiveDate = "2021-07-18";
        var expected = new SpcMesoProductEntity
        {
            Id = id,
            EffectiveDate = effectiveDate,

            AreasAffected = "a",
            Concerning = "b",
            EffectiveTime = DateTime.UtcNow,
            GraphicUrl = "c",
            Html = "d",
            Narrative = "e",
            Timestamp = DateTime.UtcNow
        };
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SpcMesoProductGet(year, id, ct))
            .ReturnsAsync(expected);
        var testable = new StormEventsSource(repo.Object);

        // Act
        var result = await testable.GetMeso(year, id, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.Id, Is.EqualTo(id));
            Assert.That(result?.EffectiveTime, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(result?.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(result?.EffectiveDate, Is.EqualTo(effectiveDate));
            Assert.That(result?.AreasAffected, Is.EqualTo("a"));
            Assert.That(result?.Concerning, Is.EqualTo("b"));
            Assert.That(result?.GraphicUrl, Is.EqualTo("c"));
            Assert.That(result?.Html, Is.EqualTo("d"));
            Assert.That(result?.Narrative, Is.EqualTo("e"));
        }
    }

    #endregion

    #region GetMesoList

    [Test]
    public async Task GetMesoListAsync_AllSteps_ValidParameters()
    {
        // Arrange
        const string effectiveDate = "2021-08-19";
        var expected = new List<SpcMesoProductEntity>();
        var ct = CancellationToken.None;
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.SpcMesoProductGetList(effectiveDate, ct))
            .ReturnsAsync(expected);
        var testable = new StormEventsSource(repo.Object);

        // Act
        var result = await testable.GetMesoList(effectiveDate, ct);

        // Assert
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region GetRadarInventory

    [Test]
    public async Task GetRadarInventory_Item_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string radarId = "KMSP";
        const string effectiveDate = "2021-07-18";
        const string bucketName = "LevelIi";
        var expected = new RadarInventoryEntity
        {
            BucketName = bucketName,
            EffectiveDate = effectiveDate,
            Id = radarId,
            Timestamp = DateTime.UtcNow,
            FileList = [string.Empty]
        };
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.RadarInventoryGet(radarId, effectiveDate, bucketName, ct))
            .ReturnsAsync(expected);
        var testable = new StormEventsSource(repo.Object);

        // Act
        var result = await testable.GetRadarInventory(radarId, effectiveDate, bucketName, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo(expected));
            Assert.That(result?.BucketName, Is.EqualTo(bucketName));
            Assert.That(result?.EffectiveDate, Is.EqualTo(effectiveDate));
            Assert.That(result?.Id, Is.EqualTo(radarId));
            Assert.That(result?.FileList, Has.Count.EqualTo(1));
            Assert.That(result?.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
        }
    }

    #endregion
}