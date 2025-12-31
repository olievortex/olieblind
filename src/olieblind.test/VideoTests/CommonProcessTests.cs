using Moq;
using olieblind.data;
using olieblind.data.Entities;
using olieblind.lib;
using olieblind.lib.Services;
using olieblind.lib.Services.Speech;
using olieblind.lib.Video;

namespace olieblind.test.VideoTests;

public class CommonProcessTests
{
    [Test]
    public async Task ImagesToMp4Async_NoExceptions_ValidInput()
    {
        // Arrange
        var ct = CancellationToken.None;
        var files = new List<string>();
        var ois = new Mock<IOlieImageService>();
        var testable = new CommonProcess(null!, ois.Object, null!);

        // Act
        var result = await testable.ImagesToMp4Async(files, string.Empty, string.Empty, string.Empty, 12, ct);

        // Assert
        Assert.That(result, Is.Not.Null);
    }

    #region PushvideoToCosmos

    [Test]
    public async Task PushVideoToCosmosAsync_NoExceptions_ValidInput()
    {
        // Arrange
        var ct = CancellationToken.None;
        var entity = new ProductVideoEntity();
        var repo = new Mock<IMyRepository>();
        repo.Setup(s => s.ProductVideoCreate(It.IsAny<ProductVideoEntity>(), ct))
            .Callback((ProductVideoEntity e, CancellationToken _) => entity = e);
        var video = Guid.NewGuid().ToString();
        var category = Guid.NewGuid().ToString();
        var poster = Guid.NewGuid().ToString();
        var title = Guid.NewGuid().ToString();
        var transcript = Guid.NewGuid().ToString();
        var videoPath = Guid.NewGuid().ToString();
        var posterPath = Guid.NewGuid().ToString();
        var testable = new CommonProcess(null!, null!, repo.Object);

        // Act
        await testable.PushVideoToMySqlAsync(video, category, poster, title, transcript, posterPath, videoPath, ct);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(entity.Id, Is.Zero);
            Assert.That(entity.Timestamp, Is.Not.EqualTo(DateTime.MinValue));
            Assert.That(entity.VideoUrl, Is.EqualTo(video));
            Assert.That(entity.PosterUrl, Is.EqualTo(poster));
            Assert.That(entity.Title, Is.EqualTo(title));
            Assert.That(entity.Category, Is.EqualTo(category));
            Assert.That(entity.Transcript, Is.EqualTo(transcript));
            Assert.That(entity.VideoLocalPath, Is.EqualTo(videoPath));
            Assert.That(entity.PosterLocalPath, Is.EqualTo(posterPath));
            Assert.That(entity.IsActive, Is.True);
        }
    }

    #endregion

    [Test]
    public void CreateStoryboard_LoopsImages_DurationLongerThanOnePass()
    {
        // Arrange
        const int imageSeconds = 5;
        var images = new List<string> { "a", "b", "c" };
        var duration = TimeSpan.FromSeconds(30);
        var testable = new CommonProcess(null!, null!, null!);

        // Act
        var result = testable.CreateStoryboard(images, imageSeconds, duration);

        // Assert
        Assert.That(result, Has.Count.EqualTo(6));
    }

    [Test]
    public void GetUploadFullPath_GeneratesCorrectPath_ValidDate()
    {
        // Arrange
        const string filePrefix = "Day1Outlook";
        const string rootFolder = "/var/spcdayone";
        const string extension = ".mp4";
        var ows = new Mock<IOlieWebService>();
        var effectiveDate = new DateTime(2018, 7, 16);
        var testable = new CommonProcess(ows.Object, null!, null!);

        // Act
        var result = testable.GetUploadFullPath(effectiveDate, rootFolder, filePrefix, extension);

        // Assert
        Assert.That(result, Is.EqualTo("/var/spcdayone/2018/07/Day1Outlook1807160000.mp4"));
    }

    [Test]
    public void GetUploadUri_GeneratesCorrectUrl_ValidDate()
    {
        // Arrange
        const string filePrefix = "Day1Outlook";
        const string extension = ".mp4";
        const string uriRoot = "https://olie.com/video";
        var ows = new Mock<IOlieWebService>();
        var effectiveDate = new DateTime(2018, 7, 16);
        var testable = new CommonProcess(ows.Object, null!, null!);

        // Act
        var result = testable.GetUploadUri(effectiveDate, uriRoot, filePrefix, extension);

        // Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(result, Is.EqualTo("https://olie.com/video/2018/07/Day1Outlook1807160000.mp4"));
        }
    }

    [Test]
    public async Task UploadVideoAsync_CopiesFile_ValidDate()
    {
        // Arrange
        const string local = "/var/tmp/file.mp4";
        const string full = "/var/www/video/2018/07/Day1Outlook1807160000.mp4";
        var localPath = string.Empty;
        var ct = CancellationToken.None;
        var ows = new Mock<IOlieWebService>();
        ows.Setup(s => s.FileWriteAllBytes(It.IsAny<string>(), It.IsAny<byte[]>(), ct))
            .Callback((string a, byte[] b, CancellationToken c) => localPath = a);
        var effectiveDate = new DateTime(2018, 7, 16);
        var testable = new CommonProcess(ows.Object, null!, null!);

        // Act
        await testable.UploadFileAsync(local, full, ct);

        // Assert
        Assert.That(localPath, Is.EqualTo("/var/www/video/2018/07/Day1Outlook1807160000.mp4"));
    }

    [Test]
    public async Task GenerateSpeechAsync_CallsApi_ValidParameters()
    {
        // Arrange
        var ct = CancellationToken.None;
        var speech = new Mock<IOlieSpeechService>();
        speech.Setup(s => s.SpeechGenerateAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), ct))
            .ReturnsAsync(TimeSpan.FromSeconds(53));
        var testable = new CommonProcess(null!, null!, null!);

        // Act
        var response = await testable.GenerateSpeechAsync(string.Empty, string.Empty, string.Empty, 5, speech.Object, ct);

        // Assert
        Assert.That(response.TotalSeconds, Is.EqualTo(55));
    }

    #region IsDaylightSavingsTime

    [Test]
    public void IsDaylightSavingsTime_ReturnsTrue_Summer()
    {
        // Arrange
        var date = new DateTime(2023, 6, 12);

        // Act
        var result = OlieCommon.IsDaylightSavingsTime(date);

        // Assert
        Assert.That(result, Is.True);
    }

    [Test]
    public void IsDaylightSavingsTime_ReturnsFalse_Winter()
    {
        // Arrange
        var date = new DateTime(2023, 2, 12);

        // Act
        var result = OlieCommon.IsDaylightSavingsTime(date);

        // Assert
        Assert.That(result, Is.False);
    }

    #endregion

    #region GetCurrentTimeZoneOffset

    [Test]
    public void GetCurrentTimeZoneOffset_ReturnsTrue_Summer()
    {
        // Arrange
        var date = new DateTime(2023, 6, 12);

        // Act
        var result = OlieCommon.GetCurrentTimeZoneOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-5));
    }

    [Test]
    public void GetCurrentTimeZoneOffset_ReturnsFalse_Winter()
    {
        // Arrange
        var date = new DateTime(2023, 2, 12);

        // Act
        var result = OlieCommon.GetCurrentTimeZoneOffset(date);

        // Assert
        Assert.That(result, Is.EqualTo(-6));
    }

    #endregion

    #region ParseSpcEffectiveDate

    [Test]
    public void ParseEffectiveDate_ThrowsException_EmptyText()
    {
        Assert.Throws<ArgumentNullException>(() => OlieCommon.ParseSpcEffectiveDate(string.Empty));
    }

    #endregion

    #region TimeZoneToOffset

    [Test]
    public void TimeZoneToOffset_CorrectOffset_BatchA()
    {
        // Act, Arrange, Assert
        using (Assert.EnterMultipleScope())
        {
            Assert.That(OlieCommon.TimeZoneToOffset("utc"), Is.Zero);
            Assert.That(OlieCommon.TimeZoneToOffset("edt"), Is.EqualTo(-4));
            Assert.That(OlieCommon.TimeZoneToOffset("est"), Is.EqualTo(-5));
            Assert.That(OlieCommon.TimeZoneToOffset("cdt"), Is.EqualTo(-5));
            Assert.That(OlieCommon.TimeZoneToOffset("cst"), Is.EqualTo(-6));
            Assert.That(OlieCommon.TimeZoneToOffset("mdt"), Is.EqualTo(-6));
            Assert.That(OlieCommon.TimeZoneToOffset("mst"), Is.EqualTo(-7));
            Assert.That(OlieCommon.TimeZoneToOffset("pdt"), Is.EqualTo(-7));
            Assert.That(OlieCommon.TimeZoneToOffset("pst"), Is.EqualTo(-8));
            Assert.Throws<ApplicationException>(() => OlieCommon.TimeZoneToOffset("dillon"));
        }
    }

    #endregion
}