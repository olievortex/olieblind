using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.Services;
using olieblind.lib.StormEvents;
using olieblind.lib.StormEvents.Models;
using System.IO.Compression;

namespace olieblind.test.StormEventsTests;

public class DatabaseBusinessTests
{
    private readonly string _html = File.ReadAllText("./Resources/StormEvents/CsvFiles.html");
    private const string EventsPath = "./Resources/StormEvents/StormEvents_Olie.csv.gz";

    private const string GzipText =
        "BEGIN_YEARMONTH,BEGIN_DAY,BEGIN_TIME,END_YEARMONTH,END_DAY,END_TIME,EPISODE_ID,EVENT_ID,STATE,STATE_FIPS,YEAR,MONTH_NAME,EVENT_TYPE,CZ_TYPE,CZ_FIPS,CZ_NAME,WFO,BEGIN_DATE_TIME,CZ_TIMEZONE,END_DATE_TIME,INJURIES_DIRECT,INJURIES_INDIRECT,DEATHS_DIRECT,DEATHS_INDIRECT,DAMAGE_PROPERTY,DAMAGE_CROPS,SOURCE,MAGNITUDE,MAGNITUDE_TYPE,FLOOD_CAUSE,CATEGORY,TOR_F_SCALE,TOR_LENGTH,TOR_WIDTH,TOR_OTHER_WFO,TOR_OTHER_CZ_STATE,TOR_OTHER_CZ_FIPS,TOR_OTHER_CZ_NAME,BEGIN_RANGE,BEGIN_AZIMUTH,BEGIN_LOCATION,END_RANGE,END_AZIMUTH,END_LOCATION,BEGIN_LAT,BEGIN_LON,END_LAT,END_LON,EPISODE_NARRATIVE,EVENT_NARRATIVE,DATA_SOURCE\r\n" +
        "202107,14,1810,202107,14,1810,159428,964226,\"IOWA\",19,2021,\"July\",\"Tornado\",\"C\",19,\"BUCHANAN\",\"DVN\",\"14-JUL-21 18:10:00\",\"CST-6\",\"14-JUL-21 18:10:00\",\"0\",\"0\",\"0\",\"0\",,,\"Emergency Manager\",,,,,\"EFU\",\"0.1\",\"10\",,,,,\"3\",\"S\",\"AURORA\",\"3\",\"S\",\"AURORA\",\"42.58\",\"-91.72\",\"42.58\",\"-91.72\",\"Severe thunderstorms developed ahead of an approaching cold front during the afternoon and evening of Wednesday, July 14.  Some of the storms produced damaging winds and numerous tornadoes.  Brief torrential rainfall was also observed with the stronger storm cells.  The areas that were hit hardest were in Buchanan, Delaware, Benton, Linn, and Jones counties in IA, where a total of 14 tornadoes occurred.  7 of the tornadoes had no visible damage.\",\"Brief touchdown in a field with no visible damage.\",\"CSV\"\r\n";

    #region ActivateSummary

    [Test]
    public async Task ActivateSummary_AllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(null!, cosmos.Object);
        var entity = new StormEventsDailySummaryEntity();

        // Act
        await testable.ActivateSummary(entity, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.IsCurrent, Is.True);
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
        }
    }

    #endregion

    #region AddDailyDetailToCosmos

    [Test]
    public async Task AddDailyDetailToCosmos_AllSteps_ValidParameters()
    {
        // Arrange
        const string id = "2021-07-18";
        var ct = CancellationToken.None;
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(null!, cosmos.Object);
        var models = new List<DailyDetailModel>();

        // Act
        await testable.AddDailyDetailToCosmos(models, id, ct);

        // Assert
        cosmos.Verify(v => v.StormEventsDailyDetailCreate(It.IsAny<List<StormEventsDailyDetailEntity>>(), ct),
            Times.Exactly(1));
    }

    #endregion

    #region AddDailySummaryToCosmos

    [Test]
    public async Task AddDailySummaryToCosmos_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        const string sourceFk = "20250401";
        var ct = CancellationToken.None;
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(null!, cosmos.Object);
        var summary = new DailySummaryModel { EffectiveDate = "2010-07-18" };

        // Act
        await testable.AddDailySummaryToCosmos(summary, sourceFk, ct);

        // Assert
        cosmos.Verify(v => v.StormEventsDailySummaryCreate(It.IsAny<StormEventsDailySummaryEntity>(), ct),
            Times.Exactly(1));
    }

    #endregion

    #region CompareDetailCount

    [Test]
    public async Task CompareDetailCount_AllSteps_ValidParameters()
    {
        // Arrange
        const string dateFk = "2021-07-18";
        const string sourceFk = "20250401";
        const int count = 42;
        var ct = CancellationToken.None;
        var cosmos = new Mock<IMyRepository>();
        cosmos.Setup(s => s.StormEventsDailyDetailCount(dateFk, sourceFk, ct))
            .ReturnsAsync(count);
        var testable = new DatabaseBusiness(null!, cosmos.Object);

        // Act
        await testable.CompareDetailCount(dateFk, sourceFk, count, ct);

        // Assert
        cosmos.Verify(v => v.StormEventsDailyDetailCount(dateFk, sourceFk, ct),
            Times.Exactly(1));
    }

    [Test]
    public void CompareDetailCount_Exception_DifferentCount()
    {
        // Arrange
        const string dateFk = "2021-07-18";
        const string sourceFk = "20250401";
        const int count = 42;
        var ct = CancellationToken.None;
        var cosmos = new Mock<IMyRepository>();
        cosmos.Setup(s => s.StormEventsDailyDetailCount(dateFk, sourceFk, ct))
            .ReturnsAsync(count + count);
        var testable = new DatabaseBusiness(null!, cosmos.Object);

        // Act, Assert
        Assert.ThrowsAsync<InvalidOperationException>(() =>
            testable.CompareDetailCount(dateFk, sourceFk, count, ct));
    }

    #endregion

    #region DatabaseParse

    [Test]
    public void DatabaseParse_Parses_ValidInput()
    {
        // Arrange
        using var stream = File.OpenRead(EventsPath);
        using var gzr = new GZipStream(stream, CompressionMode.Decompress);
        using var reader = new StreamReader(gzr);

        // Act
        var result = DatabaseBusiness.DatabaseParse(reader);

        // Assert
        Assert.That(result, Has.Count.EqualTo(1178));
    }

    #endregion

    #region DatabaseList

    [Test]
    public async Task DatabaseList_ReturnsList_ValidInput()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.ApiGetString(It.IsAny<string>(), ct))
            .ReturnsAsync(_html);
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(ows.Object, cosmos.Object);

        // Act
        var result = await testable.DatabaseList(ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Has.Count.EqualTo(75));
            Assert.That(result[0].Name, Is.EqualTo("StormEvents_details-ftp_v1.0_d1950_c20210803.csv.gz"));
            Assert.That(result[0].Year, Is.EqualTo(1950));
            Assert.That(result[0].Updated, Is.EqualTo("20210803"));
        }
    }

    #endregion

    #region DatabaseDownload

    [Test]
    public async Task DatabaseDownload_Downloads_New()
    {
        // Arrange
        StormEventsDatabaseEntity entity = new();
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        cosmos.Setup(s => s.StormEventsDatabaseGetAll(ct))
            .ReturnsAsync([]);
        cosmos.Setup(s => s.StormEventsDatabaseCreate(It.IsAny<StormEventsDatabaseEntity>(), ct))
            .Callback((StormEventsDatabaseEntity e, CancellationToken _) => entity = e);
        var testable = new DatabaseBusiness(ows.Object, cosmos.Object);
        var model = new List<DatabaseFileModel>
            { new() { Name = "StormEvents_details-ftp_v1.0_d2022_c20241121.csv.gz" } };

        // Act
        await testable.DatabaseDownload(null!, model, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(entity.Id, Is.EqualTo("20241121"));
            Assert.That(entity.Year, Is.EqualTo(2022));
            Assert.That(entity.BlobName,
                Is.EqualTo("bronze/storm-events/StormEvents_details-ftp_v1.0_d2022_c20241121.csv.gz"));
        }
    }

    [Test]
    public async Task DatabaseDownload_Skips_OldYear()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(ows.Object, cosmos.Object);
        var model = new List<DatabaseFileModel>
            { new() { Name = "StormEvents_details-ftp_v1.0_d1951_c20210803.csv.gz" } };

        // Act
        await testable.DatabaseDownload(null!, model, ct);

        // Assert
        cosmos.Verify(v => v.StormEventsDatabaseCreate(
            It.IsAny<StormEventsDatabaseEntity>(), ct), Times.Never());
    }

    [Test]
    public async Task DatabaseDownload_Skips_Existing()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int year = 2022;
        const string updated = "20241121";
        var ows = new Mock<IOlieWebService>();
        var cosmos = new Mock<IMyRepository>();
        cosmos.Setup(s => s.StormEventsDatabaseGetAll(ct))
            .ReturnsAsync([
                new StormEventsDatabaseEntity
                {
                    Year = year,
                    Id = updated + updated
                },
                new StormEventsDatabaseEntity
                {
                    Year = year,
                    Id = updated
                }
            ]);
        var testable = new DatabaseBusiness(ows.Object, cosmos.Object);
        var model = new List<DatabaseFileModel>
            { new() { Name = "StormEvents_details-ftp_v1.0_d2022_c20241121.csv.gz" } };

        // Act
        await testable.DatabaseDownload(null!, model, ct);

        // Assert
        ows.Verify(v => v.ApiGetBytes(It.IsAny<string>(), ct), Times.Never());
    }

    #endregion

    #region DatabaseUpdateActive

    [Test]
    public async Task DatabaseUpdateActive_AllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(null!, cosmos.Object);
        var entity = new StormEventsDatabaseEntity();

        // Act
        await testable.DatabaseUpdateActive(entity, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.IsActive, Is.True);
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
        }
    }

    #endregion

    #region DatabaseUpdateRowCount

    [Test]
    public async Task DatabaseUpdateRowCountAsync_AllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        const int rowCount = 42;
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(null!, cosmos.Object);
        var entity = new StormEventsDatabaseEntity();

        // Act
        await testable.DatabaseUpdateRowCount(entity, rowCount, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.RowCount, Is.EqualTo(rowCount));
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
        }
    }

    #endregion

    #region Load

    [Test]
    public async Task Load_CompletesAllSteps_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.FileReadAllTextFromGz(It.IsAny<string>(), ct))
            .ReturnsAsync(GzipText);
        var cosmos = new Mock<IMyRepository>();
        var testable = new DatabaseBusiness(ows.Object, cosmos.Object);
        var database = new StormEventsDatabaseEntity();

        // Act
        var result = await testable.DatabaseLoad(null!, database, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    #endregion
}