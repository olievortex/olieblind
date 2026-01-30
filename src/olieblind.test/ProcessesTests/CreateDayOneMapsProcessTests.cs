using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib.ForecastModels;
using olieblind.lib.Processes;
using olieblind.lib.Services;

namespace olieblind.test.ProcessesTests;

public class CreateDayOneMapsProcessTests
{
    #region Run tests

    [Test]
    public async Task RunAsync_ValidParameters_Initialized()
    {
        // Arrange
        const string baseVideoUrl = "a";
        const string destinationBaseUrl = "aa";
        const string modelForecastPath = "b";
        const string imageFolder = "bb";
        const string sourceUrl = "cc";
        const string filePrefix = "nam0618";
        const int effectiveHour = 6;
        const int forecastHour = 18;
        var dateOnly = new DateOnly(2025, 7, 14);
        var dateTime = new DateTime(2025, 7, 14, 6, 0, 0);
        var config = new Mock<IOlieConfig>();
        config.SetupGet(g => g.BaseVideoUrl).Returns(baseVideoUrl);
        config.SetupGet(g => g.ModelForecastPath).Returns(modelForecastPath);
        var nam = new Mock<INorthAmericanMesoscale>();
        nam.Setup(s => s.GetFolder(dateOnly, baseVideoUrl))
            .Returns(destinationBaseUrl);
        nam.Setup(s => s.GetFolder(dateOnly, modelForecastPath))
            .Returns(imageFolder);
        nam.Setup(s => s.GetNcepUrl(dateOnly, effectiveHour, forecastHour))
            .Returns(sourceUrl);
        var ows = new Mock<IOlieWebService>();
        var repo = new Mock<IMyRepository>();
        var testable = new CreateDayOneMapsProcess(repo.Object, nam.Object, config.Object, ows.Object);

        // Act
        await testable.Run(dateOnly, effectiveHour, forecastHour, CancellationToken.None);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(testable.EffectiveDateOnly, Is.EqualTo(dateOnly));
            Assert.That(testable.EffectiveHour, Is.EqualTo(effectiveHour));
            Assert.That(testable.ForecastHour, Is.EqualTo(forecastHour));
            Assert.That(testable.DestinationBaseUrl, Is.EqualTo(destinationBaseUrl));
            Assert.That(testable.EffectiveDateTime, Is.EqualTo(dateTime));
            Assert.That(testable.FilePrefix, Is.EqualTo(filePrefix));
            Assert.That(testable.ImageFolder, Is.EqualTo(imageFolder));
            Assert.That(testable.SourceUrl, Is.EqualTo(sourceUrl));
        }
    }

    #endregion

    #region Do tests

    [Test]
    public async Task Do_NoExceptions_Initialized()
    {
        // Arrange
        const int effectiveHour = 6;
        const int forecastHour = 18;
        var ct = CancellationToken.None;
        var dateOnly = new DateOnly(2025, 7, 14);
        var config = new Mock<IOlieConfig>();
        var nam = new Mock<INorthAmericanMesoscale>();
        var ows = new Mock<IOlieWebService>();
        var repo = new Mock<IMyRepository>();
        var testable = new CreateDayOneMapsProcess(repo.Object, nam.Object, config.Object, ows.Object)
        {
            EffectiveDateOnly = dateOnly,
            EffectiveHour = effectiveHour,
            ForecastHour = forecastHour
        };

        // Act
        await testable.Do(ct);

        // Assert
        ows.Verify(v => v.Shell(It.IsAny<IOlieConfig>(), It.IsAny<string>(), It.IsAny<string>(), ct), Times.Once);
    }

    #endregion

    #region AddToDatabase tests

    [Test]
    public async Task AddToDatabaseAsync_CreatesProductMap_Initialized()
    {
        // Arrange
        ProductMapEntity entity = new();
        const string baseVideoUrl = "a";
        const string destinationBaseUrl = "aa";
        const string modelForecastPath = "b";
        const string imageFolder = "bb";
        const string sourceUrl = "cc";
        const int effectiveHour = 6;
        const int forecastHour = 18;
        const int productId = 1;
        var ct = CancellationToken.None;
        var dateOnly = new DateOnly(2025, 7, 14);
        var dateTime = new DateTime(2025, 7, 14, 6, 0, 0);
        var config = new Mock<IOlieConfig>();
        config.SetupGet(g => g.BaseVideoUrl).Returns(baseVideoUrl);
        config.SetupGet(g => g.ModelForecastPath).Returns(modelForecastPath);
        var nam = new Mock<INorthAmericanMesoscale>();
        nam.Setup(s => s.GetFolder(dateOnly, baseVideoUrl))
            .Returns(destinationBaseUrl);
        nam.Setup(s => s.GetFolder(dateOnly, modelForecastPath))
            .Returns(imageFolder);
        nam.Setup(s => s.GetNcepUrl(dateOnly, effectiveHour, forecastHour))
            .Returns(sourceUrl);
        var ows = new Mock<IOlieWebService>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductMapCreate(It.IsAny<ProductMapEntity>(), ct))
            .Callback((ProductMapEntity e, CancellationToken _) => entity = e);
        var testable = new CreateDayOneMapsProcess(repo.Object, nam.Object, config.Object, ows.Object)
        {
            EffectiveDateOnly = dateOnly,
            EffectiveHour = effectiveHour,
            ForecastHour = forecastHour
        };

        // Act
        await testable.AddToDatabaseAsync(ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.ForecastHour, Is.EqualTo(forecastHour));
            Assert.That(entity.Effective, Is.EqualTo(dateTime));
            Assert.That(entity.SourceUrl, Is.EqualTo(sourceUrl));
            Assert.That(entity.IsActive, Is.True);
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(entity.ProductId, Is.EqualTo(productId));
        }
    }

    [Test]
    public async Task AddToDatabaseAsync_CreatesProductMapItems_Initialized()
    {
        // Arrange
        List<ProductMapItemEntity> entities = [];
        const string baseVideoUrl = "a";
        const string destinationBaseUrl = "aa";
        const string modelForecastPath = "b";
        const string imageFolder = "bb";
        const string sourceUrl = "cc";
        const int effectiveHour = 6;
        const int forecastHour = 18;
        const int geographyId = 1;
        const int parameterId = 2;
        const int mapId = 123;
        var ct = CancellationToken.None;
        var dateOnly = new DateOnly(2025, 7, 14);
        var config = new Mock<IOlieConfig>();
        config.SetupGet(g => g.BaseVideoUrl).Returns(baseVideoUrl);
        config.SetupGet(g => g.ModelForecastPath).Returns(modelForecastPath);
        var nam = new Mock<INorthAmericanMesoscale>();
        nam.Setup(s => s.GetFolder(dateOnly, baseVideoUrl))
            .Returns(destinationBaseUrl);
        nam.Setup(s => s.GetFolder(dateOnly, modelForecastPath))
            .Returns(imageFolder);
        nam.Setup(s => s.GetNcepUrl(dateOnly, effectiveHour, forecastHour))
            .Returns(sourceUrl);
        var ows = new Mock<IOlieWebService>();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductMapCreate(It.IsAny<ProductMapEntity>(), ct))
            .Callback((ProductMapEntity e, CancellationToken _) => e.Id = mapId);
        repo.Setup(s => s.ProductMapItemCreate(It.IsAny<ProductMapItemEntity>(), ct))
            .Callback((ProductMapItemEntity e, CancellationToken _) => entities.Add(e));
        var testable = new CreateDayOneMapsProcess(repo.Object, nam.Object, config.Object, ows.Object)
        {
            EffectiveDateOnly = dateOnly,
            EffectiveHour = effectiveHour,
            ForecastHour = forecastHour
        };

        // Act
        await testable.AddToDatabaseAsync(ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entities, Has.Count.EqualTo(16));
            Assert.That(entities[0].Id, Is.Zero);
            Assert.That(entities[0].GeographyId, Is.EqualTo(geographyId));
            Assert.That(entities[0].LocalPath, Is.EqualTo("bb/nam0618_Dewpoint_1.png"));
            Assert.That(entities[1].ParameterId, Is.EqualTo(parameterId));
            Assert.That(entities[0].ProductMapId, Is.EqualTo(mapId));
            Assert.That(entities[0].Url, Is.EqualTo("aa/nam0618_Dewpoint_1.png"));
            Assert.That(entities[0].IsActive, Is.True);
            Assert.That(entities[0].Timestamp, Is.Not.EqualTo(DateTime.MinValue));
        }
    }

    #endregion
}
