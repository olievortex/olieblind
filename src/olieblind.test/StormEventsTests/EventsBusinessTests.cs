using Moq;
using olieblind.data.Entities;
using olieblind.lib.StormEvents;
using olieblind.lib.StormEvents.Interfaces;

namespace olieblind.test.StormEventsTests;

public class EventsBusinessTests
{
    #region GetAnnualOverview

    [Test]
    public async Task GetAnnualOverview_Summarizes_ValidParameters()
    {
        // Arrange
        const int year = 2023;
        var ct = CancellationToken.None;
        const string date1 = "2023-07-10";
        const string date2 = "2023-07-11";
        var sourceFk = Guid.NewGuid().ToString();
        var a = new StormEventsDailySummaryEntity
            { Id = date1, SourceFk = sourceFk, F5 = 1, F4 = 2, F3 = 7, F2 = 4, F1 = 6, Wind = 3, Hail = 5, Year = year };
        var b = new StormEventsDailySummaryEntity
            { Id = date2, SourceFk = sourceFk, F5 = 0, F4 = 1, F3 = 0, F2 = 1, F1 = 5, Wind = 32, Hail = 1, Year = year };
        List<StormEventsDailySummaryEntity> events = [a, b];
        var source = new Mock<IStormEventsSource>();
        source.Setup(s => s.GetDailySummaryList(year, ct))
            .ReturnsAsync(events);
        var testable = new StormEventsBusiness(source.Object);

        // Act
        var result = await testable.GetAnnualOverview(year, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result.ExtremeTornadoes, Has.Count.EqualTo(2));
            Assert.That(result.ExtremeTornadoes[0].Id, Is.EqualTo("2023-07-10"));
            Assert.That(result.ExtremeTornadoes[0].SourceFk, Is.EqualTo(sourceFk));
            Assert.That(result.ExtremeTornadoes[0].HailCount, Is.EqualTo(5));
            Assert.That(result.ExtremeTornadoes[0].TornadoCount, Is.EqualTo(20));
            Assert.That(result.WindTop10[0].WindCount, Is.EqualTo(32));
            Assert.That(result.StrongTornadoes, Has.Count.EqualTo(2));
            Assert.That(result.HailTop10, Has.Count.EqualTo(1));
            Assert.That(result.WindTop10, Has.Count.EqualTo(2));
            Assert.That(result.Recent10, Has.Count.EqualTo(2));
            Assert.That(result.TornadoTop10, Has.Count.EqualTo(2));
        }
    }

    #endregion

    #region GetDailyDetailIdentifierByDate

    [Test]
    public async Task GetDailyDetailIdentifierByDate_ShortCircuit_NoData()
    {
        // Arrange
        var ct = CancellationToken.None;
        const string effectiveDate = "2021-07-18";
        var source = new Mock<IStormEventsSource>();
        var testable = new StormEventsBusiness(source.Object);

        // Act
        var result = await testable.GetDailyDetailIdentifierByDate(effectiveDate, ct);

        // Assert
        Assert.That(result, Is.Null);
    }

    [Test]
    public async Task GetDailyDetailIdentifierByDate_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var sourceFk = Guid.NewGuid().ToString();
        const string effectiveValue = "2021-07-18";
        const int year = 2021;
        var source = new Mock<IStormEventsSource>();
        var summary = new StormEventsDailySummaryEntity
        {
            Id = effectiveValue,
            Year = year,
            SourceFk = sourceFk,
            IsCurrent = true
        };
        source.Setup(s => s.GetDailySummaryByDate(effectiveValue, year, ct))
            .ReturnsAsync(summary);
        var testable = new StormEventsBusiness(source.Object);

        // Act
        var result = await testable.GetDailyDetailIdentifierByDate(effectiveValue, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.Not.Null);
            Assert.That(result?.SourceFk, Is.EqualTo(sourceFk));
            Assert.That(result?.EffectiveDate, Is.EqualTo(effectiveValue));
            Assert.That(result?.Year, Is.EqualTo(year));
        }
    }

    #endregion

    //#region GetDailyOverview

    //[Test]
    //public async Task GetDailyOverviewAsync_ShortCircuit_BadDate()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    const string effectiveDate = "2021-07-18";
    //    const int year = 2021;
    //    var sourceFk = Guid.NewGuid().ToString();
    //    var source = new Mock<IEventsSource>();
    //    var testable = new EventsBusiness(source.Object);

    //    // Act
    //    var result = await testable.GetDailyOverviewAsync(effectiveDate, sourceFk, year, ct);

    //    // Assert
    //    Assert.That(result, Is.Null);
    //}

    //[Test]
    //public async Task GetDailyOverviewAsync_ShortCircuit_NoSummary()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    const string effectiveValue = "2021-07-18";
    //    const int year = 2021;
    //    var effectiveDate = new DateTime(2021, 7, 18);
    //    var sourceFk = Guid.NewGuid().ToString();
    //    var source = new Mock<IEventsSource>();
    //    source.Setup(s => s.FromEffectiveDate(effectiveValue)).Returns(effectiveDate);
    //    var testable = new EventsBusiness(source.Object);

    //    // Act
    //    var result = await testable.GetDailyOverviewAsync(effectiveValue, sourceFk, year, ct);

    //    // Assert
    //    Assert.That(result, Is.Null);
    //}

    //[Test]
    //public async Task GetDailyOverviewAsync_CompletesAllSteps_ValidParameters()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    const string effectiveValue = "2021-07-18";
    //    const int year = 2021;
    //    var effectiveDate = new DateTime(2021, 7, 18);
    //    var sourceFk = Guid.NewGuid().ToString();
    //    var summary = new StormEventsDailySummaryEntity
    //    {
    //        HeadlineEventTime = effectiveDate,
    //        SatellitePathPoster = "a",
    //        SatellitePath1080 = "b"
    //    };
    //    var events = new List<StormEventsDailyDetailEntity>
    //    {
    //        new()
    //        {
    //            EventType = "Tornado",
    //            DateFk = effectiveValue,
    //            Timestamp = DateTime.UtcNow
    //        },
    //        new()
    //        {
    //            EventType = "Tornado",
    //            DateFk = effectiveValue,
    //            Timestamp = DateTime.UtcNow
    //        },
    //        new()
    //        {
    //            EventType = "Hail",
    //            DateFk = effectiveValue,
    //            Timestamp = DateTime.UtcNow
    //        },
    //        new()
    //        {
    //            EventType = "Hail",
    //            DateFk = effectiveValue,
    //            Timestamp = DateTime.UtcNow
    //        },
    //        new()
    //        {
    //            EventType = "Thunderstorm Wind",
    //            DateFk = effectiveValue,
    //            Timestamp = DateTime.UtcNow
    //        },
    //        new()
    //        {
    //            EventType = "Thunderstorm Wind",
    //            DateFk = effectiveValue,
    //            Timestamp = DateTime.UtcNow
    //        }
    //    };
    //    var source = new Mock<IEventsSource>();
    //    source.Setup(s => s.FromEffectiveDate(effectiveValue)).Returns(effectiveDate);
    //    source.Setup(s => s.GetDailySummaryAsync(effectiveValue, sourceFk, year, ct))
    //        .ReturnsAsync(summary);
    //    source.Setup(s => s.GetDailyDetailListAsync(effectiveValue, sourceFk, ct))
    //        .ReturnsAsync(events);
    //    source.Setup(s => s.GetMesoCountAsync(effectiveValue, ct))
    //        .ReturnsAsync(2);
    //    var testable = new EventsBusiness(source.Object);

    //    // Act
    //    var result = await testable.GetDailyOverviewAsync(effectiveValue, sourceFk, year, ct);

    //    // Assert
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result, Is.Not.Null);
    //        Assert.That(result?.Events, Is.EqualTo(events));
    //        Assert.That(result?.Tornadoes, Has.Count.EqualTo(2));
    //        Assert.That(result?.SatelliteDateTime, Is.EqualTo(effectiveDate));
    //        Assert.That(result?.Satellite1080Path, Is.EqualTo("b"));
    //        Assert.That(result?.SatellitePosterPath, Is.EqualTo("a"));
    //        Assert.That(result?.SatelliteAttribution, Does.StartWith("NOAA"));
    //        Assert.That(result?.MesoCount, Is.EqualTo(2));
    //    });
    //}

    //#endregion

    //#region GetSatelliteList

    //[Test]
    //public async Task GetSatelliteListAsync_Model_NoIem()
    //{
    //    // Arrange
    //    var ct = CancellationToken.None;
    //    const string effectiveDay = "2020-07-18";
    //    var source = new Mock<IEventsSource>();
    //    source.Setup(s => s.GetSatelliteListAsync(effectiveDay, ct))
    //        .ReturnsAsync([new SatelliteAwsProductEntity { BucketName = "Dillon" }]);
    //    source.Setup(s => s.GetIemSatelliteList())
    //        .Returns([new SatelliteAwsProductEntity()]);
    //    var testable = new EventsBusiness(source.Object);

    //    // Act
    //    var result = await testable.GetSatelliteListAsync(effectiveDay, ct);

    //    // Assert
    //    Assert.Multiple(() =>
    //    {
    //        Assert.That(result.AwsList, Has.Count.GreaterThan(0));
    //        Assert.That(result.IemList, Has.Count.GreaterThan(0));
    //    });
    //}

    //#endregion
}